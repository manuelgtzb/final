import type { Metadata } from "next";
import "./globals.css";
import "./language.css";
import LanguageProvider from "./components/LanguageProvider";

export const metadata: Metadata = {
  title: "Roma Digital",
  description: "Websites and landing pages designed to grow your business.",
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>
        <LanguageProvider>{children}</LanguageProvider>
      </body>
    </html>
  );
}
