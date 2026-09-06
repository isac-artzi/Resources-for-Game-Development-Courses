#!/usr/bin/env python3
"""Assemble the GitHub Pages site from each course's "Lecture Notes" folder.

Every top-level directory holding a "Lecture Notes/index.html" is published to the
site as <course-slug>/, and a landing page linking them all is generated at the
site root. Nothing in the repository is moved or duplicated: this runs at build
time inside the Pages workflow.

Usage:  python3 .github/build-site.py [output-dir]     # default: _site
"""

import html
import re
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
OUT = Path(sys.argv[1]).resolve() if len(sys.argv) > 1 else ROOT / "_site"
NOTES = "Lecture Notes"
SITE_TITLE = "Resources for Game Development Courses"
AUTHOR = "Isac Artzi"


def slug(name: str) -> str:
    return re.sub(r"-+", "-", re.sub(r"[^a-z0-9]+", "-", name.lower())).strip("-")


def page_title(path: Path) -> str:
    """The <title> of a topic page, minus the trailing course name."""
    head = path.read_text(encoding="utf-8", errors="replace")[:8000]
    m = re.search(r"<title>(.*?)</title>", head, re.S | re.I)
    if not m:
        return path.stem
    title = html.unescape(re.sub(r"\s+", " ", m.group(1))).strip()
    return title.split(" — ")[0].strip()


def topic_order(path: Path):
    m = re.search(r"(\d+)", path.stem)
    return (int(m.group(1)) if m else 999, path.stem)


def collect():
    courses = []
    for course_dir in sorted(p for p in ROOT.iterdir() if p.is_dir() and not p.name.startswith((".", "_"))):
        notes = course_dir / NOTES
        if not (notes / "index.html").is_file():
            continue
        course_slug = slug(course_dir.name)
        dest = OUT / course_slug
        for src in sorted(notes.rglob("*")):
            if src.is_dir() or src.suffix.lower() == ".md":
                continue
            target = dest / src.relative_to(notes)
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(src, target)
        # Courses name their topic pages either topic-N.html at the root of the
        # notes folder, or lessons/lesson-NN-*.html one level down.
        pages = sorted(notes.glob("topic-*.html"), key=topic_order) or sorted(
            notes.glob("lessons/lesson-*.html"), key=topic_order
        )
        topics = [
            (page_title(f), f"{course_slug}/{f.relative_to(notes).as_posix()}")
            for f in pages
        ]
        courses.append({"name": course_dir.name, "slug": course_slug, "topics": topics})
        print(f"  {course_dir.name} -> {course_slug}/ ({len(topics)} topics)")
    return courses


CSS = """
:root{--bg:#fbfbfd;--paper:#fff;--ink:#1d2233;--ink-2:#4a5169;--ink-3:#7b819a;
--line:#e3e6ef;--accent:#3b4fd8;--accent-2:#2a3aa6;--accent-soft:#eef0fd;--radius:10px;
--shadow:0 1px 2px rgba(20,25,50,.06),0 6px 20px rgba(20,25,50,.05);
--font:-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,Inter,Helvetica,Arial,sans-serif;
color-scheme:light}
*{box-sizing:border-box}
body{margin:0;background:var(--bg);color:var(--ink);font-family:var(--font);
font-size:16.5px;line-height:1.6;-webkit-font-smoothing:antialiased}
a{color:var(--accent);text-decoration:none}a:hover{text-decoration:underline}
.site-header{background:var(--paper);border-bottom:1px solid var(--line)}
.inner{max-width:900px;margin:0 auto;padding:1.4rem 1.25rem}
.site-header h1{font-size:1.5rem;margin:0;letter-spacing:-.01em}
.site-header p{margin:.45rem 0 0;color:var(--ink-2);font-size:.97rem}
main.inner{padding-top:1.8rem;padding-bottom:3rem}
.course{background:var(--paper);border:1px solid var(--line);border-radius:var(--radius);
box-shadow:var(--shadow);padding:1.4rem 1.5rem;margin-bottom:1.5rem}
.course h2{font-size:1.25rem;margin:0 0 .2rem;letter-spacing:-.01em}
.meta{font-size:.82rem;color:var(--ink-3);margin:0 0 1rem}
ol.topics{margin:0 0 1.1rem;padding-left:1.3rem}
ol.topics li{margin:.3rem 0}
.open{display:inline-block;background:var(--accent-soft);color:var(--accent-2);
font-weight:600;font-size:.92rem;padding:.4rem .9rem;border-radius:999px}
.open:hover{text-decoration:none;background:#e2e6fc}
.note{color:var(--ink-2);font-size:.92rem;margin:0 0 1.6rem}
footer{border-top:1px solid var(--line);background:var(--paper)}
footer .inner{display:flex;justify-content:space-between;gap:1rem;flex-wrap:wrap;
font-size:.85rem;color:var(--ink-3);padding:1rem 1.25rem}
"""


def render(courses) -> str:
    e = html.escape
    blocks = []
    for c in courses:
        items = "\n".join(
            f'        <li><a href="{e(href)}">{e(title)}</a></li>' for title, href in c["topics"]
        )
        blocks.append(
            f"""    <section class="course">
      <h2>{e(c['name'])}</h2>
      <p class="meta">{len(c['topics'])} topics · interactive · works offline</p>
      <ol class="topics">
{items}
      </ol>
      <a class="open" href="{e(c['slug'])}/">Open the lecture notes →</a>
    </section>"""
        )
    body = "\n".join(blocks) if blocks else "    <p>No courses published yet.</p>"
    return f"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{html.escape(SITE_TITLE)}</title>
<style>{CSS}</style>
</head>
<body>
<header class="site-header"><div class="inner">
  <h1>{html.escape(SITE_TITLE)}</h1>
  <p>Interactive lecture notes. Each topic is one self-contained page — open it here, or
     download it and read it offline.</p>
</div></header>
<main class="inner">
  <p class="note">Course activities, project skeletons, and tutorials live in the
     <a href="https://github.com/isac-artzi/Resources-for-Game-Development-Courses">repository</a>.</p>
{body}
</main>
<footer><div class="inner">
  <span>Created by {html.escape(AUTHOR)}</span>
  <span>Interactive lecture notes</span>
</div></footer>
</body>
</html>
"""


def main():
    if OUT.exists():
        shutil.rmtree(OUT)
    OUT.mkdir(parents=True)
    print(f"Building site into {OUT}")
    courses = collect()
    if not courses:
        sys.exit(f"No course folders with a '{NOTES}/index.html' were found under {ROOT}")
    (OUT / "index.html").write_text(render(courses), encoding="utf-8")
    print(f"Landing page written · {len(courses)} course(s)")


if __name__ == "__main__":
    main()
