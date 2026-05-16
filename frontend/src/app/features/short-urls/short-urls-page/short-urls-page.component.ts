import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { environment } from '../../../../environments/environment';
import { CreateShortUrlRequest, ShortUrl } from '../../../shared/models/short-url.model';
import { ShortUrlService } from '../short-url.service';
import { ShortUrlItemComponent } from '../short-url-item/short-url-item.component';
import { SiArrowLeftIcon } from '@semantic-icons/tabler-icons/outline';

@Component({
  selector: 'app-short-urls-page',
  imports: [ReactiveFormsModule, ShortUrlItemComponent, SiArrowLeftIcon],
  templateUrl: './short-urls-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShortUrlsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  readonly shortUrlService = inject(ShortUrlService);
  readonly authService = inject(AuthService);

  readonly form = this.fb.nonNullable.group({
    originalUrl: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/i)]],
  });

  readonly creating = signal(false);
  readonly deletingId = signal<string | null>(null);
  readonly formError = signal<string | null>(null);

  async ngOnInit(): Promise<void> {
    void this.authService.loadCurrentUser();
    void this.shortUrlService.loadShortUrls();
  }

  async createShortUrl(): Promise<void> {
    this.formError.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request: CreateShortUrlRequest = {
      originalUrl: this.form.controls.originalUrl.value.trim(),
    };

    this.creating.set(true);

    try {
      await this.shortUrlService.createShortUrl(request);
      this.form.reset({ originalUrl: '' });
    } catch (error: unknown) {
      this.formError.set(error instanceof Error ? error.message : 'Unable to create short URL.');
    } finally {
      this.creating.set(false);
    }
  }

  async deleteShortUrl(id: string): Promise<void> {
    this.deletingId.set(id);

    try {
      await this.shortUrlService.deleteShortUrl(id);
    } finally {
      this.deletingId.set(null);
    }
  }

  openDetails(id: string): void {
    void this.router.navigate(['/short-urls', id]);
  }

  formatCreatedAt(createdAt: string): string {
    return new Date(createdAt).toLocaleString();
  }

  get loginUrl(): string {
    return `${environment.apiBaseUrl}/Account/Login`;
  }

  get homeUrl(): string {
    return `${environment.apiBaseUrl}/`;
  }
}
