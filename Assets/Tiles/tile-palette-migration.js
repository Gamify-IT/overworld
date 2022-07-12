const yaml = require("js-yaml");
const fs = require("fs");

const inputTilePalette = "Overworld";
const outputTilePalette = "Test";

// copy all files over
fs.readdirSync(inputTilePalette)
  .filter((file) => !file.includes(".png"))
  .map((file) => `${inputTilePalette}/${file}`)
  .map((file) => ({
    origin: file,
    dest: file.split(inputTilePalette).join(outputTilePalette),
  }))
  .forEach(({ origin, dest }) => fs.copyFileSync(origin, dest));

// replace all uuids
const originalSpriteSheetFile = yaml.load(
  fs.readFileSync(`${inputTilePalette}/${inputTilePalette}.png.meta`, "utf8")
);

fs.readdirSync(outputTilePalette)
    .filter(file => file.endsWith(".asset"))
    .map((file) => `${outputTilePalette}/${file}`)
    .forEach(file => {
        const content =  yaml.load(fs.readFileSync(file));

        // rule tiles
        content?.MonoBehaviour?.m_DefaultSprite?.fileId = 
    })

console.log(originalSpriteSheetFile.TextureImporter.spriteSheet.sprites);

// const doc = yaml.load(fs.readFileSync(``, "utf8"));
// console.log(doc);
