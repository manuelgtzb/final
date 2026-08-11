export type Language = "en" | "es";

export const translations = {
  en: {
    language: { switchLabel: "Cambiar idioma a español", switchTitle: "Ver sitio en español" },
    nav: { home: "Home", services: "Services", projects: "Projects", prices: "Prices", reviews: "Reviews", contact: "Contact" },
    hero: {
      lineOne: "A WEBSITE", lineTwo: "THAT WORKS", lineThree: "WHILE YOU SLEEP",
      subtitle: "Fast. Premium. Affordable.", primaryAction: "Let's work together", secondaryAction: "View showcase",
    },
    services: {
      tag: "SERVICES", title: "Everything you need,", titleAccent: "all in one place.",
      intro: "From stunning landing pages to complete websites, I provide end-to-end solutions tailored to your goals.",
      items: [
        { title: "Landing Pages", description: "High-converting landing pages that turn visitors into customers." },
        { title: "Custom Websites", description: "Fully customized websites built for performance, scalability and style." },
        { title: "E-Commerce", description: "Online stores designed to sell more and deliver great experiences." },
        { title: "AI automation - COMING SOON", description: "AI is here to boost your sales by automating tasks. Coming soon." },
      ],
    },
    projects: {
      tag: "PROJECTS", title: "Real Demos.", titleAccent: "Real Expectations",
      description: "Your page could look like this, or even better, ready to attract potential clients.",
      titles: ["NORTAMA Landing Page", "PAMACEA", "Calendar"], openLabel: "Open project",
    },
    pricing: {
      tag: "PRICING", title: "Pick what you need,", titleAccent: "get exactly that.",
      intro: "Transparent starting prices. Final cost may vary depending on scope, integrations and content.",
      paymentLabel: "PAY SECURELY WITH", popular: "MOST POPULAR", or: "or", quote: "GET A QUOTE",
      plans: [
        {
          name: "Starter Landing", description: "A clean single-page website to present your business and capture leads.",
          features: ["Built with Next.js and React", "WhatsApp contact button", "5 sections", "Responsive design", "Basic SEO setup", "Ready in 7 days"],
        },
        {
          name: "Landing Pro", description: "A premium landing page with stronger design, animations and conversion focus.",
          features: ["Premium custom design", "Advanced animations", "7 sections", "WhatsApp chat connection", "Contact form", "Basic SEO setup", "30 days of support"],
        },
        {
          name: "Business Website", description: "A complete multi-page website for businesses that need a professional presence.",
          features: ["Multiple pages", "Custom design system", "Advanced interactions", "Contact form and WhatsApp", "Basic analytics setup", "Basic SEO setup", "Ready in 14-21 days"],
        },
      ],
    },
    reviews: {
      tag: "WHAT CLIENTS SAY", title: "Your project is in", titleAccent: "good hands.",
      customerDescription: "More than satisfied customer", profileAlt: "Profile picture of",
      verifiedLabel: "Verified client", commentAlt: "Comment from",
      stats: ["Projects completed", "Customers return", "Success rate"],
    },
    contact: {
      tag: "START YOUR PROJECT", title: "Ready to build", titleAccent: "something?",
      intro: "Tell me what you need and I'll respond with a quote and timeline.", name: "NAME",
      namePlaceholder: "Your name", email: "EMAIL", phone: "PHONE NUMBER", project: "WHAT DO YOU NEED?",
      projectPlaceholder: "Describe your website, goals and required features...", budget: "ESTIMATED BUDGET (USD)",
      hint: "Enter at least an email or phone number.", submit: "SEND THE BRIEF",
      success: "Valid form. The next step is to connect it to the API.",
    },
    footer: { copyright: "© 2026 Roma Digital. All rights reserved.", emailLabel: "Email" },
  },
  es: {
    language: { switchLabel: "Switch language to English", switchTitle: "View site in English" },
    nav: { home: "Inicio", services: "Servicios", projects: "Proyectos", prices: "Precios", reviews: "Reseñas", contact: "Contacto" },
    hero: {
      lineOne: "UN SITIO WEB", lineTwo: "QUE TRABAJA", lineThree: "MIENTRAS DUERMES",
      subtitle: "Rápido. Premium. Accesible.", primaryAction: "Trabajemos juntos", secondaryAction: "Ver proyectos",
    },
    services: {
      tag: "SERVICIOS", title: "Todo lo que necesitas,", titleAccent: "en un solo lugar.",
      intro: "Desde landing pages impactantes hasta sitios web completos, ofrezco soluciones integrales adaptadas a tus objetivos.",
      items: [
        { title: "Landing Pages", description: "Landing pages de alta conversión que transforman visitantes en clientes." },
        { title: "Sitios web personalizados", description: "Sitios totalmente personalizados, creados para ofrecer rendimiento, escalabilidad y estilo." },
        { title: "Comercio electrónico", description: "Tiendas en línea diseñadas para vender más y brindar grandes experiencias." },
        { title: "Automatización con IA - PRÓXIMAMENTE", description: "La IA está aquí para impulsar tus ventas automatizando tareas. Próximamente." },
      ],
    },
    projects: {
      tag: "PROYECTOS", title: "Demos reales.", titleAccent: "Expectativas reales",
      description: "Tu página podría verse así, o incluso mejor, y estar lista para atraer clientes potenciales.",
      titles: ["Landing Page NORTAMA", "PAMACEA", "Calendario"], openLabel: "Abrir proyecto",
    },
    pricing: {
      tag: "PRECIOS", title: "Elige lo que necesitas,", titleAccent: "recibe exactamente eso.",
      intro: "Precios iniciales transparentes. El costo final puede variar según el alcance, las integraciones y el contenido.",
      paymentLabel: "PAGA DE FORMA SEGURA CON", popular: "MÁS POPULAR", or: "o", quote: "SOLICITAR COTIZACIÓN",
      plans: [
        {
          name: "Landing Inicial", description: "Un sitio limpio de una sola página para presentar tu negocio y captar prospectos.",
          features: ["Creado con Next.js y React", "Botón de contacto de WhatsApp", "5 secciones", "Diseño responsivo", "Configuración SEO básica", "Listo en 7 días"],
        },
        {
          name: "Landing Pro", description: "Una landing page premium con mejor diseño, animaciones y enfoque en la conversión.",
          features: ["Diseño personalizado premium", "Animaciones avanzadas", "7 secciones", "Conexión con chat de WhatsApp", "Formulario de contacto", "Configuración SEO básica", "30 días de soporte"],
        },
        {
          name: "Sitio Web Empresarial", description: "Un sitio completo de varias páginas para negocios que necesitan una presencia profesional.",
          features: ["Varias páginas", "Sistema de diseño personalizado", "Interacciones avanzadas", "Formulario de contacto y WhatsApp", "Configuración de analítica básica", "Configuración SEO básica", "Listo en 14-21 días"],
        },
      ],
    },
    reviews: {
      tag: "LO QUE DICEN LOS CLIENTES", title: "Tu proyecto está en", titleAccent: "buenas manos.",
      customerDescription: "Cliente más que satisfecho", profileAlt: "Foto de perfil de",
      verifiedLabel: "Cliente verificado", commentAlt: "Comentario de",
      stats: ["Proyectos completados", "Clientes que regresan", "Tasa de éxito"],
    },
    contact: {
      tag: "INICIA TU PROYECTO", title: "¿Listo para crear", titleAccent: "algo?",
      intro: "Cuéntame qué necesitas y responderé con una cotización y un plazo estimado.", name: "NOMBRE",
      namePlaceholder: "Tu nombre", email: "CORREO ELECTRÓNICO", phone: "NÚMERO DE TELÉFONO", project: "¿QUÉ NECESITAS?",
      projectPlaceholder: "Describe tu sitio web, tus objetivos y las funciones que necesitas...", budget: "PRESUPUESTO ESTIMADO (USD)",
      hint: "Ingresa al menos un correo electrónico o número de teléfono.", submit: "ENVIAR INFORMACIÓN",
      success: "Formulario válido. El siguiente paso es conectarlo con la API.",
    },
    footer: { copyright: "© 2026 Roma Digital. Todos los derechos reservados.", emailLabel: "Correo electrónico" },
  },
} as const;
