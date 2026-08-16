import type { Metadata } from "next";
import "./globals.css";
import "./language.css";
import LanguageProvider from "./components/LanguageProvider";

export const metadata: Metadata = {
  metadataBase: new URL("https://romalabs.xyz"),
  title: "Roma Labs | Diseño y desarrollo web",
  description: "Diseñamos landing pages y sitios web modernos para negocios de Tampico, Ciudad Madero y Altamira",
  alternates: {
    canonical: "/",
  },

  icons: {
    icon: "/romaIcon.png",
    apple: "/apple-icon.png",
  },
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="es">
      <body>
        <LanguageProvider>{children}</LanguageProvider>
      </body>
    </html>
  );
}
