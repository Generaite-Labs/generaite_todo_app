# ADR: Selection of Tailwind CSS with DaisyUI for Frontend Styling

## Status

Accepted

## Context

Our project requires a styling solution that is flexible, efficient, and allows for rapid UI development. We also need a framework that facilitates easy creation of mockups and portability of designs across different platforms. The team's existing familiarity with certain tools is a consideration in this decision.

## Decision

We have decided to use Tailwind CSS with DaisyUI for frontend styling in our project.

## Rationale

Tailwind CSS with DaisyUI was chosen for the following reasons:

1. Familiarity: The team already has experience with Tailwind and DaisyUI, reducing the learning curve and enabling faster development.

2. Cross-platform Compatibility: These tools work well across different platforms, allowing for easy creation and porting of mockups and designs.

3. Rapid Prototyping: Tailwind's utility-first approach, combined with DaisyUI's pre-built components, enables quick prototyping and iteration of UI designs.

4. Customization: Tailwind offers a high degree of customization, allowing us to tailor the design system to our specific needs.

5. Performance: Tailwind's purge feature can significantly reduce the final CSS bundle size, potentially improving load times.

6. Consistency: Using a utility-first CSS framework can help maintain design consistency across the application.

7. Component Library: DaisyUI provides a set of pre-styled components that can speed up development while still allowing for customization.

8. Active Community: Both Tailwind and DaisyUI have active communities, ensuring ongoing support and resources.

9. Integration with Blazor: While not native to the .NET ecosystem, there are established patterns for using Tailwind with Blazor applications.

### Alternatives Considered

1. Bootstrap:
   - While widely used, it doesn't offer the same level of utility-first flexibility as Tailwind.

2. Material UI:
   - Rejected due to potential conflicts with our desire for a unique design language.

3. CSS Modules or Styled Components:
   - These approaches are more commonly used in React ecosystems and may not integrate as smoothly with Blazor.

4. Native CSS:
   - While offering maximum control, it would require more time to develop and maintain custom styles.

## Consequences

### Positive

- Rapid UI development and prototyping capabilities.
- Consistent design system across the application.
- Ability to easily create and port mockups across platforms.
- Leveraging the team's existing knowledge of Tailwind and DaisyUI.

### Negative

- Potential for long class names in HTML, which some developers find less readable.
- Learning curve for team members not familiar with utility-first CSS approaches.
- May require additional setup to integrate smoothly with Blazor and our build process.

### Neutral

- Need to ensure that our use of Tailwind and DaisyUI aligns well with Blazor's component model.
- Regular updates to Tailwind and DaisyUI may require periodic adjustments to our styles.

## References

- Tailwind CSS documentation: https://tailwindcss.com/docs
- DaisyUI documentation: https://daisyui.com/
- Integrating Tailwind CSS with Blazor: https://chrissainty.com/integrating-tailwind-css-with-blazor/
- Utility-First CSS concept: https://tailwindcss.com/docs/utility-first