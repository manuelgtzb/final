import type { MetadataRoute } from "next";

export default function robots(): MetadataRoute.Robots {
  return {
    rules: {
      userAgent: "*",
      allow: "/",
    },
    sitemap: "https://romalabs.xyz/sitemap.xml",
    host: "https://romalabs.xyz",
  };
}
