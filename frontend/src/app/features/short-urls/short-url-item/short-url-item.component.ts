import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  EventEmitter,
  inject,
} from '@angular/core';
import { AuthService } from '../../../core/auth/auth.service';
import { ShortUrl } from '../../../shared/models/short-url.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'tr[app-short-url-item]',
  templateUrl: './short-url-item.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class ShortUrlItemComponent {
  readonly authService = inject(AuthService);

  @Input() item!: ShortUrl;
  @Input() deletingId: string | null = null;
  @Output() delete = new EventEmitter<string>();
  @Output() detail = new EventEmitter<string>();

  onDelete(): void {
    this.delete.emit(this.item.id);
  }

  onDetail(): void {
    this.detail.emit(this.item.id);
  }

  formatCreatedAt(createdAt?: string): string {
    return createdAt ? new Date(createdAt).toLocaleString() : '-';
  }

  get redirectUrl(): string {
    return `${environment.apiBaseUrl}${this.item.shortUrl}`;
  }
}
