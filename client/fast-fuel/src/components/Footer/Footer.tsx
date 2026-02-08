import './Footer.css';

export const Footer = () => {
  return (
    <footer className="footer">
      <p>© {new Date().getFullYear()} Fast Fuel. All rights reserved.</p>
    </footer>
  );
};
