# Landing Final

Monorepo de Roma Labs:

- Backend: ASP.NET Core 8 en `api/`
- Frontend: Next.js en `web/`
- Pruebas: xUnit en `api.Tests/`

## Requisitos

- .NET SDK 8
- Node.js 20 o superior
- npm

## Instalación

```bash
dotnet restore LandingFinal.sln
npm install
```

## Configuración local

La configuración pública del frontend se guarda en `web/.env.local`:

```text
NEXT_PUBLIC_API_URL=http://localhost:5182
NEXT_PUBLIC_TURNSTILE_SITE_KEY=tu_site_key_publica
```

Los secretos del backend no deben guardarse en archivos del repositorio. Configúralos con .NET User Secrets:

```bash
dotnet user-secrets set "Resend:ApiKey" "tu_api_key" --project api
dotnet user-secrets set "ContactEmail:To" "destino@tu-dominio.com" --project api
dotnet user-secrets set "ContactEmail:From" "ROMA LABS <contacto@tu-dominio-verificado.com>" --project api
dotnet user-secrets set "ContactEmail:LogoUrl" "https://tu-dominio.com/LogoRoma.png" --project api
dotnet user-secrets set "Turnstile:SecretKey" "tu_secret_key" --project api
```

En producción usa las variables equivalentes:

```text
Resend__ApiKey
ContactEmail__To
ContactEmail__From
ContactEmail__LogoUrl
Turnstile__SecretKey
AllowedOrigins__0
AllowedOrigins__1
```

`ContactEmail__From` debe usar un dominio verificado en Resend. `ContactEmail__LogoUrl` debe ser una URL HTTPS pública.

## Desarrollo

Terminal 1:

```bash
npm run dev:api
```

La API queda en `http://localhost:5182`.

Terminal 2:

```bash
npm run dev:web
```

Next.js queda en `http://localhost:3000`.

## Verificación

```bash
dotnet test LandingFinal.sln
npm run lint:web
npm run build:web
```

## Endpoints

- `GET /api/health`
- `GET /api/content`
- `POST /api/contact`

## Produccion: Vercel

1. Importa el repositorio en un proyecto nuevo de Vercel.
2. En `Root Directory`, selecciona `web`.
3. Conserva `Next.js` como Framework Preset.
4. Selecciona `24.x` como version de Node.js.
5. Usa `npm run build` como Build Command.
6. No agregues secretos del backend a Vercel.

En `Settings > Environment Variables`, agrega para `Production`:

```text
NEXT_PUBLIC_API_URL=https://api.romalabs.xyz
NEXT_PUBLIC_TURNSTILE_SITE_KEY=tu_site_key_publica
```

En `Settings > Domains`, agrega:

- `romalabs.xyz`
- `www.romalabs.xyz`

`web/public/LogoRoma.png` se publica como `/LogoRoma.png`.

## Produccion: Render

1. Crea un `Web Service` conectado al mismo repositorio.
2. Selecciona `Docker` como Language.
3. Configura `api` como Root Directory.
4. Usa `Dockerfile` como Dockerfile Path.
5. Configura `/api/health` como Health Check Path.
6. Deja Docker Command vacio para usar el `ENTRYPOINT`.

En `Environment`, agrega:

```text
Resend__ApiKey=tu_nueva_clave_resend
ContactEmail__From=Roma Labs <projects@mail.romalabs.xyz>
ContactEmail__To=tu_correo_destino
ContactEmail__LogoUrl=https://romalabs.xyz/LogoRoma.png
Turnstile__SecretKey=tu_secret_key
AllowedOrigins__0=https://romalabs.xyz
AllowedOrigins__1=https://www.romalabs.xyz
```

Render proporciona `PORT`; el contenedor usa `10000` como valor predeterminado.
La API escucha en `0.0.0.0` y rechaza cualquier otro origen CORS en produccion.
No agregues estos secretos como argumentos de compilacion Docker.

## DNS: Cloudflare

1. Agrega primero los dominios en Vercel y Render para obtener sus valores DNS exactos.
2. En `SSL/TLS > Overview`, usa modo `Full`.
3. Crea estos registros en la zona `romalabs.xyz`:

- `@`: registro A con el valor indicado por Vercel.
- `www`: CNAME con el destino indicado por Vercel.
- `api`: CNAME al subdominio `onrender.com` de tu servicio Render.

4. Usa `DNS only` mientras Vercel y Render verifican dominios y emiten certificados.
5. Elimina registros AAAA que entren en conflicto con el dominio de Render.
6. Verifica `https://api.romalabs.xyz/api/health` antes de publicar el frontend.
No apuntes `api.romalabs.xyz` a Vercel.
