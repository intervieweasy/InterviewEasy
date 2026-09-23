# Interview Easy topic pages and branding

## What will change
- Replace every technology pill in the menu with a real link to its own dedicated topic page.
- Improve the menu with clearer visual hierarchy, larger clickable rows, category counts, and an arrow cue while preserving the Frosted Glass style.
- Add a reusable topic page that shows the selected technology name, category, interview preparation overview, and clearly marked areas for questions, explanations, videos, examples, and practice.
- Generate an Interview Easy logo image, use it consistently in the site navigation, and derive a compact favicon from the same mark.
- Add unique page titles, descriptions, Open Graph details, and relevant keyword metadata to the homepage, Careers, Contact, and every topic page.
- Keep all existing homepage sections, careers listings, contact content, videos, and workflows unchanged except where navigation branding is updated.

## Technical details
- Use one dynamic TanStack route (`/topics/$slug`) backed by a shared topic catalogue, so all menu entries have distinct URLs without duplicating dozens of files.
- Validate unknown topic slugs and show the site’s not-found screen rather than displaying incorrect content.
- Store the generated logo in the project assets and create a 64px public favicon; update the root document favicon reference.
- Verify representative topic links, metadata, logo rendering, desktop layout, and mobile layout in the running preview.
