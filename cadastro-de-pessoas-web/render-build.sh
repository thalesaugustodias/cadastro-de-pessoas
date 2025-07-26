# Build command for Render.com
# This should be used in the Render dashboard

# Install dependencies and build
npm ci --only=production
npm run build

# Or for development dependencies (if needed)
# npm install
# npm run build