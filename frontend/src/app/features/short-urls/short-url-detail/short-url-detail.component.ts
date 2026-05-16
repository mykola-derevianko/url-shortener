import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ShortUrlService } from '../short-url.service';
import { ShortUrl } from '../../../shared/models/short-url.model';
import { environment } from '../../../../environments/environment';
import { SiArrowLeftIcon } from '@semantic-icons/tabler-icons/outline';

@Component({
  selector: 'app-short-url-detail',
  imports: [RouterLink, SiArrowLeftIcon],
  templateUrl: './short-url-detail.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShortUrlDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  readonly shortUrlService = inject(ShortUrlService);
  readonly shortUrl = signal<ShortUrl | null>(null);
  readonly error = signal<string | null>(null);

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error.set('Missing short URL id.');
      return;
    }

    try {
      this.shortUrl.set(await this.shortUrlService.loadShortUrl(id));
    } catch (error: unknown) {
      this.error.set(error instanceof Error ? error.message : 'Unable to load short URL.');
    }
  }

  formatCreatedAt(createdAt: string): string {
    return createdAt ? new Date(createdAt).toLocaleString() : '-';
  }

  formatRedirect(shortPath?: string | null): string {
    return shortPath ? `${environment.apiBaseUrl}/${shortPath}` : '';
  }
}
