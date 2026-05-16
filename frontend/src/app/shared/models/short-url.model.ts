export interface ShortUrl {
  id: string;
  originalUrl: string;
  shortUrl: string;
  createdByUserId: string;
  createdAt: string;
}

export interface CreateShortUrlRequest {
  originalUrl: string;
}

export interface ShortUrlListResponse {
  items: ShortUrl[];
}
