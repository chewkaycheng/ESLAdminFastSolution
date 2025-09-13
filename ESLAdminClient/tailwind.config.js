/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.razor",  // Scans all Client .razor files (e.g., Counter.razor)
    "./**/*.cshtml", // For any server pages
    "./wwwroot/**/*.html"  // Static HTML
  ],
  // v4 defaults to purging; add safelist for core classes if needed (Step 3)
  safelist: [
    // Add more as needed, or use regex: { pattern: /text-(blue|red)-.*/ }
  ],
  theme: {
    extend: {},  // Your customizations
  },
  plugins: [],
};