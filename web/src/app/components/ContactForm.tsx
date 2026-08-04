"use client";

import { useRef, useState } from "react";
import { useLanguage } from "./LanguageProvider";
import { translations } from "../translations";
import { contactFormTranslations } from "./contactFormTranslations";
import styles from "./ContactForm.module.css";

type SubmissionStatus = "idle" | "sending" | "success" | "error";
type ContactResponse = { success?: boolean; code?: string; message?: string };

declare global {
  interface Window {
    turnstile?: { reset: (widgetId?: string) => void };
  }
}

const apiBaseUrl = process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "") ?? "";
const turnstileSiteKey = process.env.NEXT_PUBLIC_TURNSTILE_SITE_KEY ?? "";
const hasPublicConfiguration = Boolean(apiBaseUrl && turnstileSiteKey);

export default function ContactForm() {
  const { language } = useLanguage();
  const t = translations[language].contact;
  const formT = contactFormTranslations[language];
  const submittingRef = useRef(false);
  const idempotencyKeyRef = useRef<string | null>(null);
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [status, setStatus] = useState<SubmissionStatus>("idle");
  const [statusMessage, setStatusMessage] = useState("");

  function handleFormInput() {
    idempotencyKeyRef.current = null;
    if (status !== "sending" && status !== "idle") {
      setStatus("idle");
      setStatusMessage("");
    }
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submittingRef.current) return;

    const form = event.currentTarget;
    const formData = new FormData(form);
    const turnstileToken = String(formData.get("cf-turnstile-response") ?? "");

    if (!hasPublicConfiguration) {
      setStatus("error");
      setStatusMessage(formT.configurationError);
      return;
    }

    if (!turnstileToken) {
      setStatus("error");
      setStatusMessage(formT.verificationRequired);
      return;
    }

    submittingRef.current = true;
    setStatus("sending");
    setStatusMessage(formT.sending);
    const idempotencyKey = idempotencyKeyRef.current ?? crypto.randomUUID();
    idempotencyKeyRef.current = idempotencyKey;

    try {
      const response = await fetch(`${apiBaseUrl}/api/contact`, {
        method: "POST",
        headers: { "Content-Type": "application/json", "Idempotency-Key": idempotencyKey },
        body: JSON.stringify({
          name: formData.get("name"),
          email: formData.get("email"),
          phone: formData.get("phone"),
          project: formData.get("project"),
          budget: Number(formData.get("budget")),
          currency: formData.get("currency"),
          turnstileToken,
        }),
      });

      const result = await readResponse(response);
      if (!response.ok) {
        setStatus("error");
        setStatusMessage(getErrorMessage(response.status, result.code, formT));
        return;
      }

      form.reset();
      setEmail("");
      setPhone("");
      idempotencyKeyRef.current = null;
      setStatus("success");
      setStatusMessage(formT.success);
    } catch {
      setStatus("error");
      setStatusMessage(formT.error);
    } finally {
      submittingRef.current = false;
      window.turnstile?.reset();
    }
  }

  return (
    <section id="contact" className="contactFormSection">
      <script src="https://challenges.cloudflare.com/turnstile/v0/api.js" async defer></script>
      <div className="contactFormHeader">
        <p className="sectionTag">{t.tag}</p>
        <h2>{t.title} <span>{t.titleAccent}</span></h2>
        <p>{t.intro}</p>
      </div>

      <form className="projectForm" onSubmit={handleSubmit} onInput={handleFormInput}>
        <label>
          {t.name}
          <input name="name" type="text" placeholder={t.namePlaceholder} minLength={2} maxLength={100} autoComplete="name" required />
        </label>
        <div className="contactFields">
          <label>
            {t.email}
            <input name="email" type="email" placeholder="you@email.com" maxLength={254} autoComplete="email" value={email} onChange={(event) => setEmail(event.target.value)} required={!phone.trim()} />
          </label>
          <label>
            {t.phone}
            <input name="phone" type="tel" placeholder="+52 833 123 4567" minLength={7} maxLength={30} autoComplete="tel" value={phone} onChange={(event) => setPhone(event.target.value)} required={!email.trim()} />
          </label>
        </div>
        <p className="contactHint">{t.hint}</p>
        <label>
          {t.project}
          <textarea name="project" placeholder={t.projectPlaceholder} minLength={10} maxLength={3000} required />
        </label>

        <div className={styles.budgetFields}>
          <label>
            {formT.budget}
            <input name="budget" type="number" min="0.01" max="100000000" step="0.01" placeholder="300" required />
          </label>
          <label>
            {formT.currency}
            <select name="currency" defaultValue="USD" required>
              <option value="MXN">MXN</option>
              <option value="USD">USD</option>
            </select>
          </label>
        </div>

        <div className={styles.turnstile}>
          {turnstileSiteKey && <div className="cf-turnstile" data-sitekey={turnstileSiteKey} data-theme="dark" data-size="flexible" />}
        </div>

        {statusMessage && (
          <p className={`${styles.status} ${status === "success" ? styles.success : status === "error" ? styles.error : ""}`} role={status === "error" ? "alert" : "status"} aria-live="polite">
            {statusMessage}
          </p>
        )}

        <button type="submit" className={`formButton ${styles.button}`} disabled={status === "sending" || !hasPublicConfiguration}>
          {status === "sending" ? formT.sending : `${t.submit} →`}
        </button>
      </form>
    </section>
  );
}

async function readResponse(response: Response): Promise<ContactResponse> {
  try {
    return await response.json() as ContactResponse;
  } catch {
    return {};
  }
}

function getErrorMessage(status: number, code: string | undefined, messages: typeof contactFormTranslations.en) {
  if (status === 429 || code === "rate_limited") return messages.rateLimited;
  if (code === "turnstile_invalid") return messages.verificationError;
  if (status === 400) return messages.validationError;
  return messages.error;
}
