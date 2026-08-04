import type { Language } from "../translations";

export const contactFormTranslations: Record<Language, {
  budget: string;
  currency: string;
  sending: string;
  success: string;
  error: string;
  validationError: string;
  verificationRequired: string;
  verificationError: string;
  rateLimited: string;
  configurationError: string;
}> = {
  en: {
    budget: "ESTIMATED BUDGET",
    currency: "CURRENCY",
    sending: "SENDING...",
    success: "Your project request was sent successfully.",
    error: "We couldn't send your request. Please try again.",
    validationError: "Please review the form fields and try again.",
    verificationRequired: "Please complete the human verification.",
    verificationError: "Verification expired or failed. Please try again.",
    rateLimited: "Too many requests. Please wait before trying again.",
    configurationError: "The contact form is not configured yet.",
  },
  es: {
    budget: "PRESUPUESTO ESTIMADO",
    currency: "MONEDA",
    sending: "ENVIANDO...",
    success: "Tu solicitud de proyecto se envió correctamente.",
    error: "No pudimos enviar tu solicitud. Inténtalo de nuevo.",
    validationError: "Revisa los campos del formulario e inténtalo de nuevo.",
    verificationRequired: "Completa la verificación humana.",
    verificationError: "La verificación expiró o falló. Inténtalo de nuevo.",
    rateLimited: "Hay demasiadas solicitudes. Espera antes de intentarlo nuevamente.",
    configurationError: "El formulario de contacto todavía no está configurado.",
  },
};
