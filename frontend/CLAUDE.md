# Qwestr Web (Blazor Server)

Фронтенд SaaS-платформы для онбординга с геймификацией. Отдельный проект, взаимодействует с backend через HTTP API.

## Tech Stack

- Blazor Server (.NET 9)
- MudBlazor — единственная UI-библиотека, никаких других UI-фреймворков
- HTTP-клиент для связи с Qwestr.Api
- JWT-аутентификация (access token в памяти, refresh token в HttpOnly cookie)

## UI Design Style — Clean Gamified Professional

Ориентир: Duolingo — геймификация мотивирует, но интерфейс чистый и не перегруженный.

- Light, airy interface: white card surfaces (#FFFFFF) on soft gray background (#F5F5F5)

- One bold accent color for primary actions and active states, neutral grays for everything else

- Generous whitespace and padding — never cluttered, content breathes

- Large rounded corners (12-16px) on cards and containers for approachable feel

- Gamification elements (points, badges, levels) styled clean and minimal — no cartoonish graphics

- Progress indicators: thin gradient progress bars, circular percentage rings, subtle animations

- Metric cards in grid layout: big bold number + small label underneath, icon in colored circle

- Achievement cards: simple icon + title + short description, muted background tint when locked

- Typography hierarchy: bold large numbers for stats, medium weight for headings, regular for body

- Micro-interactions: smooth transitions on hover/click, gentle fade-ins for loaded content

- Mobile-first card-based layouts that naturally reflow to multi-column on desktop

- No decorative gradients in content area — reserve gradients only for hero/welcome banners

- Consistent icon style: outlined Material Design icons, not mixed styles

  ```
  DISTILLED_AESTHETICS_PROMPT = """
  <frontend_aesthetics>
  You tend to converge toward generic, "on distribution" outputs. In frontend design, this creates what users call the "AI slop" aesthetic. Avoid this: make creative, distinctive frontends that surprise and delight. Focus on:
  
  Typography: Choose fonts that are beautiful, unique, and interesting. Avoid generic fonts like Arial and Inter; opt instead for distinctive choices that elevate the frontend's aesthetics.
  
  Color & Theme: Commit to a cohesive aesthetic. Use CSS variables for consistency. Dominant colors with sharp accents outperform timid, evenly-distributed palettes. Draw from IDE themes and cultural aesthetics for inspiration.
  
  Motion: Use animations for effects and micro-interactions. Prioritize CSS-only solutions for HTML. Use Motion library for React when available. Focus on high-impact moments: one well-orchestrated page load with staggered reveals (animation-delay) creates more delight than scattered micro-interactions.
  
  Backgrounds: Create atmosphere and depth rather than defaulting to solid colors. Layer CSS gradients, use geometric patterns, or add contextual effects that match the overall aesthetic.
  
  Avoid generic AI-generated aesthetics:
  - Overused font families (Inter, Roboto, Arial, system fonts)
  - Clichéd color schemes (particularly purple gradients on white backgrounds)
  - Predictable layouts and component patterns
  - Cookie-cutter design that lacks context-specific character
  
  Interpret creatively and make unexpected choices that feel genuinely designed for the context. Vary between light and dark themes, different fonts, different aesthetics. You still tend to converge on common choices (Space Grotesk, for example) across generations. Avoid this: it is critical that you think outside the box!
  </frontend_aesthetics>
  """
  ```

## Component Rules

- Использовать ТОЛЬКО MudBlazor компоненты (MudCard, MudAvatar, MudIcon, MudText, MudPaper и т.д.)
- Кастомный CSS только если MudBlazor не покрывает потребность — и только в scoped .razor.css
- Никакого inline CSS в .razor файлах
- Никаких сторонних JS-библиотек без явного запроса

## Project Structure

```
Shared/
  MainLayout.razor            — основной layout (шапка + sidebar/bottom nav)
  MainLayout.razor.css
  AuthLayout.razor            — layout для логина/регистрации (без навигации)
  Components/
    TopBar.razor              — верхняя шапка (логотип, очки, аватар)
    SideNav.razor             — sidebar для desktop (MudDrawer MiniVariant)
    BottomNav.razor           — нижняя навигация для mobile
Pages/
  Dashboard.razor
  Tests.razor
  Games.razor
  Knowledge.razor
  Auth/
    Login.razor
    Register.razor
Services/
  ApiClient.cs                — HTTP-клиент к backend API
  AuthStateProvider.cs        — кастомный AuthenticationStateProvider
```

## Layout

- Desktop (>960px): сворачиваемый sidebar слева (MudDrawer MiniVariant — иконки в свёрнутом, иконки + текст в развёрнутом). Шапка сверху на всю ширину.
- Mobile (<960px): sidebar скрыт, навигация через фиксированную нижнюю панель (4 вкладки)
- Контент: максимальная ширина 1100px, центрирован
- Два layout: MainLayout (авторизованные страницы) и AuthLayout (логин/регистрация)

## Important Rules

- НЕ смешивать стили UI-библиотек — только MudBlazor
- НЕ создавать компоненты-обёртки вокруг MudBlazor без причины
- НЕ хранить access token в localStorage/sessionStorage — только в памяти
- Каждая страница — отдельный .razor файл в соответствующей папке
- Адаптивность через MudHidden, Breakpoint, а не через media queries вручную
