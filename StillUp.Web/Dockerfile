# Build stage
FROM oven/bun:1 AS build
WORKDIR /app

# Install dependencies first for better layer caching
COPY package.json bun.lock* ./
RUN bun install --frozen-lockfile

COPY . .

ARG BACKEND_URL=http://stillup-api:5370
ENV BACKEND_URL=$BACKEND_URL

RUN bun run build

# Runtime stage
FROM oven/bun:1-slim AS runtime
WORKDIR /app

COPY --from=build /app/build ./build
COPY --from=build /app/package.json .

RUN bun install --production

ENV PORT=3000
EXPOSE 3000

CMD ["bun", "build/index.js"]
