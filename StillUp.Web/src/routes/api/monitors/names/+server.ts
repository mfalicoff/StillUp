import { BACKEND_URL } from "$env/static/private";
import { json } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";

export const GET: RequestHandler = async () => {
  let response: Response;
  try {
    response = await fetch(new URL("/api/monitors/names", BACKEND_URL).toString());
  } catch {
    return json({ error: "Monitor service unavailable" }, { status: 503 });
  }

  if (!response.ok) {
    return json({ error: "Failed to retrieve names" }, { status: response.status });
  }

  let data: unknown;
  try {
    data = await response.json();
  } catch {
    return json({ error: "Unexpected response" }, { status: 502 });
  }

  return json(data);
};
