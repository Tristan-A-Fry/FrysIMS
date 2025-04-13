
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}", // <- include your src
  ],
  theme: {
    extend: {
      colors: {
        fryblue: "#21c9ff"
      }
    },
  },
  plugins: [],
}

