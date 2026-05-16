import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable, inject, signal } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { environment } from "../../../environments/environment";
import {
  CreateShortUrlRequest,
  ShortUrl,
} from "../../shared/models/short-url.model";
import { ApiErrorResponse } from "../../shared/models/api-error.model";

@Injectable({ providedIn: "root" })
export class ShortUrlService {
  private readonly http = inject(HttpClient);
  private readonly shortUrlsState = signal<ShortUrl[]>([]);
  private readonly loadingState = signal(false);
  private readonly errorState = signal<string | null>(null);
  private readonly selectedShortUrlState = signal<ShortUrl | null>(null);

  readonly shortUrls = this.shortUrlsState.asReadonly();
  readonly loading = this.loadingState.asReadonly();
  readonly error = this.errorState.asReadonly();
  readonly selectedShortUrl = this.selectedShortUrlState.asReadonly();

  async loadShortUrls(): Promise<void> {
    this.loadingState.set(true);
    this.errorState.set(null);

    try {
      const items = await firstValueFrom(
        this.http.get<ShortUrl[]>(`${environment.apiBaseUrl}/api/short-urls`, {
          withCredentials: true,
        }),
      );

      this.shortUrlsState.set(items);
    } catch (error: unknown) {
      this.errorState.set(this.describeError(error));
    } finally {
      this.loadingState.set(false);
    }
  }

  async loadShortUrl(id: string): Promise<ShortUrl> {
    this.loadingState.set(true);
    this.errorState.set(null);

    try {
      const item = await firstValueFrom(
        this.http.get<ShortUrl>(
          `${environment.apiBaseUrl}/api/short-urls/${id}`,
          { withCredentials: true },
        ),
      );

      this.selectedShortUrlState.set(item);
      return item;
    } catch (error: unknown) {
      this.errorState.set(this.describeError(error));
      throw error;
    } finally {
      this.loadingState.set(false);
    }
  }

  async createShortUrl(request: CreateShortUrlRequest): Promise<ShortUrl> {
    this.errorState.set(null);

    try {
      const createdShortUrl = await firstValueFrom(
        this.http.post<ShortUrl>(
          `${environment.apiBaseUrl}/api/short-urls`,
          request,
          { withCredentials: true },
        ),
      );

      this.shortUrlsState.update((current) => [createdShortUrl, ...current]);
      return createdShortUrl;
    } catch (error: unknown) {
      const message = this.describeError(error);
      this.errorState.set(message);
      throw new Error(message);
    }
  }

  async deleteShortUrl(id: string): Promise<void> {
    this.errorState.set(null);

    try {
      await firstValueFrom(
        this.http.delete<void>(
          `${environment.apiBaseUrl}/api/short-urls/${id}`,
          { withCredentials: true },
        ),
      );

      this.shortUrlsState.update((current) =>
        current.filter((item) => item.id !== id),
      );
    } catch (error: unknown) {
      const message = this.describeError(error);
      this.errorState.set(message);
      throw new Error(message);
    }
  }

  private describeError(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 409) {
        return "A short URL already exists for that destination.";
      }

      const apiError = error.error as ApiErrorResponse | string | null;

      if (typeof apiError === "string") {
        return apiError;
      }

      return (
        apiError?.message ??
        apiError?.detail ??
        error.message ??
        "Unable to process short URL request."
      );
    }

    return "Unable to process short URL request.";
  }
}
