import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

async function getWeather() {
  try {
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5277"}/api/WeatherForecast`, {
      cache: "no-store",
    });

    if (!res.ok) {
      throw new Error(`Request failed with status ${res.status}`);
    }

    return (await res.json()) as {
      date: string;
      temperatureC: number;
      summary?: string;
    }[];
  } catch {
    return [];
  }
}

export default async function Home() {
  const forecasts = await getWeather();

  return (
    <main className="flex min-h-screen flex-col items-center justify-start bg-background p-8 text-foreground">
      <div className="w-full max-w-2xl space-y-4">
        <header className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold tracking-tight">Clean Architecture Monorepo</h1>
            <p className="text-sm text-muted-foreground">Sample data from .NET backend (WeatherForecast).</p>
          </div>
          <Button variant="outline" size="sm" asChild>
            <Link href="/">Refresh</Link>
          </Button>
        </header>

        <Card>
          <CardHeader>
            <CardTitle>Forecast</CardTitle>
          </CardHeader>
          <CardContent>
            {forecasts.length === 0 ? (
              <p className="text-sm text-muted-foreground">No data (is the API running?).</p>
            ) : (
              <ul className="space-y-1 text-sm">
                {forecasts.map((f, idx) => (
                  <li key={idx} className="flex items-center justify-between">
                    <span className="font-medium">{f.date}</span>
                    <span>
                      {f.temperatureC} °C{f.summary ? ` · ${f.summary}` : ""}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
