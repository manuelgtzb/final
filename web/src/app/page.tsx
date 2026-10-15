"use client";

import Image from "next/image";
import { Brain, CreditCard, Languages, MonitorDot, ShoppingCart } from "lucide-react";
import ContactForm from "./components/ContactForm";
import { useLanguage } from "./components/LanguageProvider";
import Reviews from "./components/Reviews";
import { translations } from "./translations";

const serviceIcons = [CreditCard, MonitorDot, ShoppingCart, Brain];

const projects = [
  { image: "/Nortama.png", url: "https://demo-landpage.netlify.app/" },
  { image: "/Pamacea.png", url: "https://www.pamacea.com.mx/" },
  { image: "/calendario.png", url: "https://calendario-chi-orpin.vercel.app/" },
];

const prices = [
  { priceMx: "$4,500 MXN", priceUsd: "$280 USD", popular: false },
  { priceMx: "$8,500 MXN", priceUsd: "$500 USD", popular: true },
  { priceMx: "From $15,000 MXN", priceUsd: "From $870 USD", popular: false },
] as const;

const CONTACT_EMAIL = "supportromalabs@gmail.com";

// TODO: pega aquí tus enlaces reales de Instagram y TikTok
const INSTAGRAM_URL = "https://www.instagram.com/romalabs_/";
const TIKTOK_URL = "https://www.tiktok.com/@romalabss";

export default function Home() {
  const { language, toggleLanguage } = useLanguage();
  const t = translations[language];
  const gmailComposeUrl = `https://mail.google.com/mail/?view=cm&fs=1&to=${CONTACT_EMAIL}&su=${encodeURIComponent(t.mailButton.subject)}&body=${encodeURIComponent(t.mailButton.body)}`;
  const localizedPrice = (price: string) => language === "es" ? price.replace("From", "Desde") : price;

  return (
    <main className="page">
      <header className="header">
        <input type="checkbox" id="menuToggle" className="menuToggle" />
        <label htmlFor="menuToggle" className="menuButton" aria-label={language === "es" ? "Abrir menú" : "Open menu"}>
          <span></span><span></span><span></span>
        </label>

        <a href="#" className="logo"><img src="/LogoRoma.png" alt="Roma Labs" /></a>

        <nav className="nav">
          <a href="#">{t.nav.home}</a>
          <a href="#services">{t.nav.services}</a>
          <a href="#projects">{t.nav.projects}</a>
          <a href="#prices">{t.nav.prices}</a>
          <a href="#reviews">{t.nav.reviews}</a>
          <a href="#contact">{t.nav.contact}</a>
        </nav>

        <button type="button" className="langBtn" onClick={toggleLanguage} aria-label={t.language.switchLabel} title={t.language.switchTitle}>
          <Languages size={16} aria-hidden="true" />
          <span className={language === "en" ? "activeLanguage" : ""}>EN</span>
          <span aria-hidden="true">/</span>
          <span className={language === "es" ? "activeLanguage" : ""}>ES</span>
        </button>
      </header>

      <section className="hero">
        <div className="heroContent">
          <h1><span>{t.hero.lineOne}</span>{t.hero.lineTwo}<strong>{t.hero.lineThree}</strong></h1>
          <p>{t.hero.subtitle}</p>
          <div className="heroActions">
            <a href="#contact" className="btnPrimary">{t.hero.primaryAction} <span>→</span></a>
            <a href="#projects" className="btnSecondary">{t.hero.secondaryAction}</a>
          </div>
        </div>
      </section>

      <section id="services" className="services">
        <div className="servicesTop">
          <div>
            <p className="sectionTag">{t.services.tag}</p>
            <h2>{t.services.title}<span>{t.services.titleAccent}</span></h2>
          </div>
          <p className="servicesIntro">{t.services.intro}</p>
        </div>
        <div className="servicesGrid">
          {t.services.items.map((service, index) => {
            const Icon = serviceIcons[index];
            return (
              <article className="serviceCard" key={service.title}>
                <Icon className="serviceIcon" size={28} strokeWidth={1.8} />
                <h3>{service.title}</h3><p>{service.description}</p>
                <a href="#contact" aria-label={`${t.nav.contact}: ${service.title}`}>→</a>
              </article>
            );
          })}
        </div>
      </section>

      <section id="projects" className="projects">
        <div className="projectsHeader">
          <p className="sectionTag">{t.projects.tag}</p>
          <h2>{t.projects.title} <span>{t.projects.titleAccent}</span></h2>
        </div>
        <div className="projectsCarousel">
          <div className="projectsTrack">
            {[...projects, ...projects].map((project, index) => {
              const title = t.projects.titles[index % projects.length];
              return (
                <a href={project.url} target="_blank" rel="noreferrer" className="projectCard" key={`${title}-${index}`} aria-label={`${t.projects.openLabel}: ${title}`}>
                  <img src={project.image} alt={title} />
                  <div className="projectInfo"><h3>{title}</h3><span>→</span></div>
                </a>
              );
            })}
          </div>
        </div>
        <p className="projectsText">{t.projects.description}</p>
      </section>

      <section id="prices" className="pricing">
        <div className="pricingTop">
          <div className="pricingHeader">
            <p className="sectionTag">{t.pricing.tag}</p>
            <h2>{t.pricing.title}<span>{t.pricing.titleAccent}</span></h2>
            <p>{t.pricing.intro}</p>
          </div>
          <div className="paymentMethods">
            <p>{t.pricing.paymentLabel}</p>
            <div className="paymentLogos">
              <Image src="/paypal.png" alt="PayPal" width={90} height={36} />
              <Image src="/visa.png" alt="Visa" width={70} height={36} />
              <Image src="/master.png" alt="Mastercard" width={60} height={36} />
            </div>
          </div>
        </div>

        <a href="#contact" className="promoBanner">
          <span className="promoTag">{t.pricing.promo.tag}</span>
          <span className="promoMain">
            <span className="promoPlan">{t.pricing.promo.plan}</span>
            <strong className="promoPrice">{t.pricing.promo.price}</strong>
            <s className="promoOldPrice">{t.pricing.promo.oldPrice}</s>
          </span>
          <span className="promoCta">{t.pricing.promo.cta} →</span>
        </a>

        <div className="pricingGrid">
          {t.pricing.plans.map((plan, index) => {
            const price = prices[index];
            return (
              <article className={`priceCard ${price.popular ? "popular" : ""}`} key={plan.name}>
                {price.popular && <span className="popularBadge">{t.pricing.popular}</span>}
                <h3>{plan.name}</h3><p className="priceDescription">{plan.description}</p>
                <div className="priceRow">
                  <strong>{localizedPrice(price.priceMx)}</strong><span>{t.pricing.or}</span><strong>{localizedPrice(price.priceUsd)}</strong>
                </div>
                <ul>{plan.features.map((feature) => <li key={feature}>{feature}</li>)}</ul>
                <a href="#contact" className="priceButton">{t.pricing.quote}</a>
              </article>
            );
          })}
        </div>

        <Reviews />
        <ContactForm />

        <footer className="footer">
          <a href="#" className="footerLogo"><img src="/Romafooter.png" alt="Roma Digital" /></a>
          <p>{t.footer.copyright}</p>
          <div className="footerSocials">
            <a href="https://discord.com/users/779824297530097675" aria-label="Discord">dc</a>
            <a href="mailto:supportromalabs@gmail.com" aria-label={t.footer.emailLabel}>✉</a>
            <a href={INSTAGRAM_URL} target="_blank" rel="noreferrer" className="socialRound" aria-label="Instagram">
              <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                <rect x="3" y="3" width="18" height="18" rx="5" />
                <circle cx="12" cy="12" r="4" />
                <circle cx="17.5" cy="6.5" r="0.6" fill="currentColor" />
              </svg>
            </a>
            <a href={TIKTOK_URL} target="_blank" rel="noreferrer" className="socialRound" aria-label="TikTok">
              <svg viewBox="0 0 24 24" width="19" height="19" fill="currentColor" aria-hidden="true">
                <path d="M16.6 5.82A4.28 4.28 0 0 1 15.54 3h-3.09v12.4a2.59 2.59 0 0 1-2.59 2.5c-1.42 0-2.6-1.16-2.6-2.6 0-1.72 1.66-3.01 3.37-2.48V9.66c-3.45-.46-6.47 2.22-6.47 5.64 0 3.33 2.76 5.7 5.69 5.7 3.14 0 5.69-2.55 5.69-5.7V9.01a7.35 7.35 0 0 0 4.3 1.38V7.3s-1.88.09-3.24-1.48Z" />
              </svg>
            </a>
          </div>
        </footer>
      </section>

      <a href={gmailComposeUrl} target="_blank" rel="noreferrer" className="floatingMail" aria-label={t.mailButton.label} title={t.mailButton.label}>
        <img src="/btncorreo.png" alt="" width={64} height={64} />
      </a>
    </main>
  );
}
