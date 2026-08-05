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
  { image: "/projects/Nortama.png", url: "https://demo-landpage.netlify.app/" },
  { image: "/projects/Nortama.png", url: "https://demo-landpage.netlify.app/" },
];

const prices = [
  { priceMx: "$4,500 MXN", priceUsd: "$280 USD", popular: false },
  { priceMx: "$8,500 MXN", priceUsd: "$500 USD", popular: true },
  { priceMx: "From $15,000 MXN", priceUsd: "From $870 USD", popular: false },
] as const;

export default function Home() {
  const { language, toggleLanguage } = useLanguage();
  const t = translations[language];
  const localizedPrice = (price: string) => language === "es" ? price.replace("From", "Desde") : price;

  return (
    <main className="page">
      <header className="header">
        <input type="checkbox" id="menuToggle" className="menuToggle" />
        <label htmlFor="menuToggle" className="menuButton" aria-label={language === "es" ? "Abrir menú" : "Open menu"}>
          <span></span><span></span><span></span>
        </label>

        <a href="#" className="logo"><img src="/LogoRoma.png" alt="Roma Digital" /></a>

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
            <a href="mailto:ricardoemmanuelgutierrezb@gmail.com" aria-label={t.footer.emailLabel}>✉</a>
          </div>
        </footer>
      </section>
    </main>
  );
}
