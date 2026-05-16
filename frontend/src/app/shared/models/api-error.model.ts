export interface ApiErrorResponse {
  message?: string;
  title?: string;
  detail?: string;
  errors?: Record<string, string[] | string>;
}
