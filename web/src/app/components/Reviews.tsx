"use client";

import Image from "next/image";
import { useEffect, useRef, useState } from "react";
import { BadgeCheckIcon } from "lucide-react";
import { useLanguage } from "./LanguageProvider";
import { translations } from "../translations";

const reviews = [
  { name: "King", avatar: "/reviews/king-avatar.png", comment: "/reviews/King.png" },
  { name: "Prachi", avatar: "/reviews/none-avatar.png", comment: "/reviews/prachi.jpeg" },
  { name: "Crusty", avatar: "/reviews/crusty-avatar.png", comment: "/reviews/crusty.jpeg" },
  { name: "Jail", avatar: "/reviews/jail.avatar.png", comment: "/reviews/Jail.png" },
  { name: "James", avatar: "/reviews/james-avatar.png", comment: "/reviews/james.png" },
  { name: "Tristan", avatar: "/reviews/tristan-avatar.png", comment: "/reviews/tristan.png" },
  { name: "Countcarlo", avatar: "/reviews/count-avatar.png", comment: "/reviews/countcarlo.png" },
];

function Counter({ number, suffix = "" }: { number: number; suffix?: string }) {
  const element = useRef<HTMLSpanElement>(null);
  const [value, setValue] = useState(0);

  useEffect(() => {
    const node = element.current;
    if (!node) return;
    let animationFrame = 0;

    const observer = new IntersectionObserver(([entry]) => {
      cancelAnimationFrame(animationFrame);
      if (!entry.isIntersecting) {
        setValue(0);
        return;
      }

      const startTime = performance.now();
      const duration = 1400;
      const animate = (currentTime: number) => {
        const progress = Math.min((currentTime - startTime) / duration, 1);
        setValue(Math.round(number * (1 - Math.pow(1 - progress, 3))));
        if (progress < 1) animationFrame = requestAnimationFrame(animate);
      };
      animationFrame = requestAnimationFrame(animate);
    }, { threshold: 0.4 });

    observer.observe(node);
    return () => {
      observer.disconnect();
      cancelAnimationFrame(animationFrame);
    };
  }, [number]);

  return <span ref={element}>{value}{suffix}</span>;
}

export default function Reviews() {
  const { language } = useLanguage();
  const t = translations[language].reviews;
  const repeatedReviews = [...reviews, ...reviews];

  return (
    <section id="reviews" className="reviews">
      <p className="sectionTag">{t.tag}</p>
      <h2>{t.title} <span>{t.titleAccent}</span></h2>
      <div className="reviewsViewport">
        <div className="reviewsTrack">
          {repeatedReviews.map((review, index) => (
            <article className="reviewItem" key={`${review.name}-${index}`}>
              <div className="reviewClient">
                <Image src={review.avatar} alt={`${t.profileAlt} ${review.name}`} width={52} height={52} />
                <div>
                  <h3>{review.name}<BadgeCheckIcon size={18} aria-label={t.verifiedLabel} /></h3>
                  <p>{t.customerDescription}</p>
                </div>
              </div>
              <div className="commentFrame">
                <Image src={review.comment} alt={`${t.commentAlt} ${review.name}`} fill sizes="(max-width: 620px) 90vw, 380px" />
              </div>
            </article>
          ))}
        </div>
      </div>
      <div className="reviewStats">
        <div><strong><Counter number={40} suffix="+" /></strong><p>{t.stats[0]}</p></div>
        <div><strong><Counter number={70} suffix="%" /></strong><p>{t.stats[1]}</p></div>
        <div><strong><Counter number={95} suffix="%" /></strong><p>{t.stats[2]}</p></div>
      </div>
    </section>
  );
}
