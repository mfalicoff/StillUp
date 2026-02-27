import { BACKEND_URL } from "$env/static/private";
import type { RequestHandler } from "./$types";

export const GET: RequestHandler = async ({ request }) => {
  const backendUrl = new URL("/api/monitors/stream", BACKEND_URL);

  let backendResponse: Response;
  try {
    backendResponse = await fetch(backendUrl.toString(), {
      signal: request.signal,
      headers: { Accept: "text/event-stream" },
    });
  } catch (err) {
    if (err instanceof DOMException && err.name === "AbortError") {
      return new Response(null, { status: 499 });
    }
    return new Response("Monitor stream unavailable", { status: 503 });
  }

  if (!backendResponse.ok) {
    console.error(`Monitor stream returned ${backendResponse.status}`);
    return new Response("Monitor stream unavailable", { status: backendResponse.status });
  }

  return new Response(backendResponse.body, {
    headers: {
      "Content-Type": "text/event-stream",
      "Cache-Control": "no-cache",
      Connection: "keep-alive",
      "X-Accel-Buffering": "no",
    },
  });
};
