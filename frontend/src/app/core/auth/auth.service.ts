import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiErrorResponse } from '../../shared/models/api-error.model';
import { CurrentUser, MeResponse } from '../../shared/models/auth.model';
import { ShortUrl } from '../../shared/models/short-url.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly currentUserState = signal<CurrentUser | null>(null);
  private readonly loadingState = signal(false);
  private readonly errorState = signal<string | null>(null);

  readonly currentUser = this.currentUserState.asReadonly();
  readonly loading = this.loadingState.asReadonly();
  readonly error = this.errorState.asReadonly();
  readonly isAdmin = computed(() => this.currentUserState()?.role === 'Admin');

  async loadCurrentUser(): Promise<void> {
    if (this.loadingState()) {
      return;
    }

    this.loadingState.set(true);
    this.errorState.set(null);

    try {
      const response = await firstValueFrom(
        this.http.get<MeResponse>(`${environment.apiBaseUrl}/api/account/me`, {
          withCredentials: true,
        }),
      );

      const role = this.getSingleRole(response.role);
      this.currentUserState.set({
        id: response.userId ?? '',
        email: response.email ?? '',
        role,
      });
    } catch (error: unknown) {
      this.currentUserState.set(null);
      if (!this.isUnauthorizedError(error)) {
        this.errorState.set(this.describeError(error));
      }
    } finally {
      this.loadingState.set(false);
    }
  }

  canDelete(shortUrl: ShortUrl): boolean {
    const currentUser = this.currentUserState();

    if (!currentUser) {
      return false;
    }
    return currentUser.role === 'Admin' || currentUser.id == shortUrl.createdByUserId;
  }

  private getSingleRole(role: string | string[] | undefined): 'Admin' | 'User' | null {
    if (!role) {
      return null;
    }

    const raw = Array.isArray(role) ? role[0] : role;
    if (raw === 'Admin') return 'Admin';
    if (raw === 'User') return 'User';
    return null;
  }

  private isUnauthorizedError(error: unknown): boolean {
    return error instanceof HttpErrorResponse && (error.status === 401 || error.status === 403);
  }

  private describeError(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      const apiError = error.error as ApiErrorResponse | string | null;

      if (typeof apiError === 'string') {
        return apiError;
      }

      return (
        apiError?.message ?? apiError?.detail ?? error.message ?? 'Unable to load current user.'
      );
    }

    return 'Unable to load current user.';
  }
}
