import argparse
import shutil
from collections import deque
from pathlib import Path

from PIL import Image


DEFAULT_GENERATED_ROOT = Path.home() / ".codex" / "generated_images"


def find_latest_generated_png() -> Path:
    if not DEFAULT_GENERATED_ROOT.exists():
        raise FileNotFoundError(f"Generated image folder not found: {DEFAULT_GENERATED_ROOT}")

    pngs = [path for path in DEFAULT_GENERATED_ROOT.rglob("*.png") if path.is_file()]
    if not pngs:
        raise FileNotFoundError(f"No PNG files found under: {DEFAULT_GENERATED_ROOT}")

    return max(pngs, key=lambda path: path.stat().st_mtime)


def remove_chroma_key(source: Path, output: Path, threshold: int, soft_range: int) -> None:
    image = Image.open(source).convert("RGBA")
    pixels = image.load()
    width, height = image.size

    border_samples = []
    for x in range(width):
        border_samples.append(pixels[x, 0][:3])
        border_samples.append(pixels[x, height - 1][:3])
    for y in range(height):
        border_samples.append(pixels[0, y][:3])
        border_samples.append(pixels[width - 1, y][:3])

    key = tuple(round(sum(color[i] for color in border_samples) / len(border_samples)) for i in range(3))

    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            distance = ((r - key[0]) ** 2 + (g - key[1]) ** 2 + (b - key[2]) ** 2) ** 0.5

            if distance <= threshold:
                pixels[x, y] = (r, g, b, 0)
            elif distance < soft_range:
                alpha = int(255 * (distance - threshold) / max(1, soft_range - threshold))
                pixels[x, y] = (r, g, b, min(a, alpha))

    output.parent.mkdir(parents=True, exist_ok=True)
    image.save(output)


def find_frame_components(image: Image.Image, frame_count: int, alpha_threshold: int, min_area: int):
    alpha = image.getchannel("A")
    pixels = alpha.load()
    width, height = image.size
    visited = bytearray(width * height)
    components = []

    for y in range(height):
        for x in range(width):
            idx = y * width + x
            if visited[idx] or pixels[x, y] <= alpha_threshold:
                continue

            queue = deque([(x, y)])
            visited[idx] = 1
            min_x = max_x = x
            min_y = max_y = y
            count = 0

            while queue:
                current_x, current_y = queue.popleft()
                count += 1
                min_x = min(min_x, current_x)
                max_x = max(max_x, current_x)
                min_y = min(min_y, current_y)
                max_y = max(max_y, current_y)

                for next_x, next_y in (
                    (current_x + 1, current_y),
                    (current_x - 1, current_y),
                    (current_x, current_y + 1),
                    (current_x, current_y - 1),
                ):
                    if next_x < 0 or next_y < 0 or next_x >= width or next_y >= height:
                        continue

                    next_idx = next_y * width + next_x
                    if visited[next_idx] or pixels[next_x, next_y] <= alpha_threshold:
                        continue

                    visited[next_idx] = 1
                    queue.append((next_x, next_y))

            if count >= min_area:
                components.append((min_x, min_y, max_x + 1, max_y + 1, count))

    components = sorted(components, key=lambda box: box[4], reverse=True)[:frame_count]
    return sorted(components, key=lambda box: (box[0] + box[2]) / 2)


def pad_sprite_sheet(
    source: Path,
    output: Path,
    frame_count: int,
    cell_width: int,
    cell_height: int,
    bottom_padding: int,
    max_width_padding: int,
    max_height_padding: int,
) -> None:
    image = Image.open(source).convert("RGBA")
    components = find_frame_components(image, frame_count, alpha_threshold=10, min_area=1000)

    if len(components) != frame_count:
        raise RuntimeError(f"Expected {frame_count} frame components, found {len(components)}.")

    sheet = Image.new("RGBA", (cell_width * frame_count, cell_height), (0, 0, 0, 0))
    max_frame_width = max(1, cell_width - max_width_padding)
    max_frame_height = max(1, cell_height - max_height_padding)

    for index, box in enumerate(components):
        min_x, min_y, max_x, max_y, _ = box
        frame = image.crop((min_x, min_y, max_x, max_y))
        frame_width, frame_height = frame.size

        scale = min(max_frame_width / frame_width, max_frame_height / frame_height, 1.0)
        if scale < 1.0:
            frame = frame.resize(
                (round(frame_width * scale), round(frame_height * scale)),
                Image.Resampling.LANCZOS,
            )
            frame_width, frame_height = frame.size

        x = index * cell_width + (cell_width - frame_width) // 2
        y = cell_height - frame_height - bottom_padding
        sheet.alpha_composite(frame, (x, y))

    output.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(output)


def main() -> None:
    parser = argparse.ArgumentParser(description="Prepare imagegen creature idle sprite sheets for Unity.")
    input_group = parser.add_mutually_exclusive_group(required=True)
    input_group.add_argument("--input", type=Path, help="Generated green-background PNG to process.")
    input_group.add_argument("--latest-generated", action="store_true", help="Use newest PNG under ~/.codex/generated_images.")
    parser.add_argument("--out-dir", type=Path, required=True, help="Project output directory.")
    parser.add_argument("--name", required=True, help="Base file name without extension.")
    parser.add_argument("--frames", type=int, default=8)
    parser.add_argument("--cell-width", type=int, default=384)
    parser.add_argument("--cell-height", type=int, default=512)
    parser.add_argument("--bottom-padding", type=int, default=38)
    parser.add_argument("--chroma-threshold", type=int, default=18)
    parser.add_argument("--chroma-soft-range", type=int, default=90)
    args = parser.parse_args()

    source = find_latest_generated_png() if args.latest_generated else args.input
    source = source.resolve()

    args.out_dir.mkdir(parents=True, exist_ok=True)
    raw_path = args.out_dir / f"{args.name}_raw.png"
    transparent_path = args.out_dir / f"{args.name}_transparent.png"
    padded_path = args.out_dir / f"{args.name}_padded_{args.cell_width}x{args.cell_height}.png"

    shutil.copy2(source, raw_path)
    remove_chroma_key(raw_path, transparent_path, args.chroma_threshold, args.chroma_soft_range)
    pad_sprite_sheet(
        transparent_path,
        padded_path,
        args.frames,
        args.cell_width,
        args.cell_height,
        args.bottom_padding,
        max_width_padding=44,
        max_height_padding=72,
    )

    print(f"Source: {source}")
    print(f"Raw copy: {raw_path}")
    print(f"Transparent: {transparent_path}")
    print(f"Padded: {padded_path}")
    print(f"Unity slice: Grid By Cell Size {args.cell_width} x {args.cell_height}")


if __name__ == "__main__":
    main()
