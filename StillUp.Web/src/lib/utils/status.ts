import type { MonitorEntry, ServiceStatus, StatusCategory } from "$lib/types";

// Maps .NET HttpStatusCode enum names to numeric status codes
const STATUS_CODE_MAP: Record<string, number> = {
  // 1xx
  Continue: 100,
  SwitchingProtocols: 101,
  // 2xx
  OK: 200,
  Created: 201,
  Accepted: 202,
  NonAuthoritativeInformation: 203,
  NoContent: 204,
  ResetContent: 205,
  PartialContent: 206,
  MultiStatus: 207,
  AlreadyReported: 208,
  IMUsed: 226,
  // 3xx
  MultipleChoices: 300,
  Ambiguous: 300,
  MovedPermanently: 301,
  Moved: 301,
  Found: 302,
  Redirect: 302,
  SeeOther: 303,
  RedirectMethod: 303,
  NotModified: 304,
  UseProxy: 305,
  Unused: 306,
  TemporaryRedirect: 307,
  RedirectKeepVerb: 307,
  PermanentRedirect: 308,
  // 4xx
  BadRequest: 400,
  Unauthorized: 401,
  PaymentRequired: 402,
  Forbidden: 403,
  NotFound: 404,
  MethodNotAllowed: 405,
  NotAcceptable: 406,
  ProxyAuthenticationRequired: 407,
  RequestTimeout: 408,
  Conflict: 409,
  Gone: 410,
  LengthRequired: 411,
  PreconditionFailed: 412,
  RequestEntityTooLarge: 413,
  RequestUriTooLong: 414,
  UnsupportedMediaType: 415,
  RequestedRangeNotSatisfiable: 416,
  ExpectationFailed: 417,
  MisdirectedRequest: 421,
  UnprocessableEntity: 422,
  UnprocessableContent: 422,
  Locked: 423,
  FailedDependency: 424,
  UpgradeRequired: 426,
  PreconditionRequired: 428,
  TooManyRequests: 429,
  RequestHeaderFieldsTooLarge: 431,
  UnavailableForLegalReasons: 451,
  // 5xx
  InternalServerError: 500,
  NotImplemented: 501,
  BadGateway: 502,
  ServiceUnavailable: 503,
  GatewayTimeout: 504,
  HttpVersionNotSupported: 505,
  VariantAlsoNegotiates: 506,
  InsufficientStorage: 507,
  LoopDetected: 508,
  NotExtended: 510,
  NetworkAuthenticationRequired: 511,
};

export function categorizeStatusCode(statusCode: string): StatusCategory {
  const numeric = STATUS_CODE_MAP[statusCode];
  if (numeric === undefined) return "unknown";
  if (numeric >= 200 && numeric < 300) return "success";
  if (numeric >= 300 && numeric < 400) return "redirect";
  if (numeric >= 400 && numeric < 500) return "client-error";
  if (numeric >= 500) return "server-error";
  return "unknown";
}

export function categoryToColorClass(cat: StatusCategory): string {
  switch (cat) {
    case "success":
      return "bg-emerald-500";
    case "redirect":
      return "bg-yellow-400";
    case "client-error":
      return "bg-orange-500";
    case "server-error":
      return "bg-red-500";
    default:
      return "bg-gray-300 dark:bg-gray-600";
  }
}

export function categoryToLabel(cat: StatusCategory): string {
  switch (cat) {
    case "success":
      return "Operational";
    case "redirect":
      return "Redirect";
    case "client-error":
      return "Client Error";
    case "server-error":
      return "Server Error";
    default:
      return "No data";
  }
}

export function deriveServiceStatus(entries: MonitorEntry[]): ServiceStatus {
  if (entries.length === 0) return "unknown";
  const cat = categorizeStatusCode(entries[0].statusCode);
  if (cat === "success") return "up";
  if (cat === "redirect") return "degraded";
  return "down";
}
