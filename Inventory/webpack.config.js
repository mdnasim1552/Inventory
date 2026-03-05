import path from 'path';
import { fileURLToPath } from 'url';
import MiniCssExtractPlugin from 'mini-css-extract-plugin';


const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export default {
  mode: 'production',
    entry: {
        coronaryDiagram: path.resolve(__dirname, './wwwroot/js/src/coronaryDiagram/index.js'),
        /*diagram2: path.resolve(__dirname, '.wwwroot/js/src/diagram2/index.js')*/
    },
  output: {
      filename: '[name].bundle.js',
      path: path.resolve(__dirname, 'wwwroot/js/dist'),
      clean: true
  },
  module: {
    rules: [
      {
        test: /\.css$/i,
        use: [
          MiniCssExtractPlugin.loader, // 👈 extracts to file
          'css-loader'
        ],
      },
    ],
  },
  plugins: [
    new MiniCssExtractPlugin({
        filename: '[name].bundle.css'
    }),
  ],
};
