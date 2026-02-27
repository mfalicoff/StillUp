import { BACKEND_URL } from "$env/static/private";
import { json } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";

export const GET: RequestHandler = async ({ params, url }) => {
  const hours = parseInt(params.hours, 10);
  if (isNaN(hours) || hours <= 0) {
    return json({ error: "Invalid hours" }, { status: 400 });
  }

  const serviceName = url.searchParams.get("serviceName");

  const backendUrl = new URL(`/api/monitors/recent/${hours}`, BACKEND_URL);
  if (serviceName) backendUrl.searchParams.set("serviceName", serviceName);

  let response: Response;
  try {
    response = await fetch(backendUrl.toString());
  } catch {
    return json({ error: "Monitor service unavailable" }, { status: 503 });
  }

  if (!response.ok) {
    return json({ error: "Failed to retrieve monitor data" }, { status: response.status });
  }

  let data: unknown;
  try {
    data = await response.json();
  } catch {
    return json({ error: "Unexpected response" }, { status: 502 });
  }

  return json(data);
};
