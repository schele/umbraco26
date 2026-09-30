import { LitElement, css, html, nothing, unsafeCSS } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UmbImagingRepository } from '@umbraco-cms/backoffice/imaging';

/**
 * Block Editor Custom View for the `carouselBlock` element type. Shows the
 * picked images (`images` property) as a browsable slideshow in the backoffice,
 * styled like the Bootstrap 5.3 carousel rendered by Views/Partials/carousel.cshtml.
 * Clicking the slide opens the block editor as usual.
 */

// Same crop as the frontend partial.
const IMAGE_WIDTH = 1920;
const IMAGE_HEIGHT = 640;

// Bootstrap 5.3 carousel control icons.
const PREV_ICON = `url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23fff'%3e%3cpath d='M11.354 1.646a.5.5 0 0 1 0 .708L5.707 8l5.647 5.646a.5.5 0 0 1-.708.708l-6-6a.5.5 0 0 1 0-.708l6-6a.5.5 0 0 1 .708 0'/%3e%3c/svg%3e")`;
const NEXT_ICON = `url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23fff'%3e%3cpath d='M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708'/%3e%3c/svg%3e")`;

export class CarouselPreviewElement extends UmbElementMixin(LitElement) {
    static properties = {
        content: { attribute: false },
        settings: { attribute: false },
        label: { attribute: false },
        config: { attribute: false },
        _urls: { state: true },
        _index: { state: true },
        _loading: { state: true },
    };

    #imagingRepository = new UmbImagingRepository(this);
    #requestedKeys = '';

    constructor() {
        super();
        this._urls = [];
        this._index = 0;
        this._loading = false;
    }

    updated(changed) {
        if (changed.has('content')) this.#loadImages();
    }

    async #loadImages() {
        const mediaKeys = (this.content?.images ?? []).map((x) => x.mediaKey).filter(Boolean);
        const requestKey = mediaKeys.join(',');
        if (requestKey === this.#requestedKeys) return;
        this.#requestedKeys = requestKey;

        if (!mediaKeys.length) {
            this._urls = [];
            this._index = 0;
            return;
        }

        this._loading = true;
        const { data } = await this.#imagingRepository.requestResizedItems(mediaKeys, {
            width: IMAGE_WIDTH,
            height: IMAGE_HEIGHT,
            mode: 'Crop',
        });
        // Ignore stale responses if the picked images changed while loading.
        if (requestKey !== this.#requestedKeys) return;

        const urlByKey = new Map((data ?? []).map((x) => [x.unique, x.url]));
        this._urls = mediaKeys.map((key) => urlByKey.get(key)).filter(Boolean);
        this._index = Math.min(this._index, Math.max(this._urls.length - 1, 0));
        this._loading = false;
    }

    #go(event, step) {
        // Don't let navigation clicks open the block editor.
        event.stopPropagation();
        const count = this._urls.length;
        this._index = (this._index + step + count) % count;
    }

    #goTo(event, index) {
        event.stopPropagation();
        this._index = index;
    }

    render() {
        return html`<div class="carousel">${this.#renderBody()}</div>`;
    }

    #renderBody() {
        const editPath = this.config?.editContentPath ?? nothing;

        if (this._loading && !this._urls.length) {
            return html`<div class="placeholder"><uui-loader></uui-loader></div>`;
        }
        if (!this._urls.length) {
            return html`<a class="placeholder" href=${editPath}>
                <uui-icon name="icon-pictures-alt-2"></uui-icon>
                <span>${this.label ?? 'Carousel'} — no images selected</span>
            </a>`;
        }

        return html`
            <a class="carousel-inner" href=${editPath} style="transform: translateX(-${this._index * 100}%)">
                ${this._urls.map((url, i) => html`<img src=${url} alt="" loading="lazy" draggable="false" aria-hidden=${i !== this._index} />`)}
            </a>
            ${this._urls.length > 1
                ? html`
                      <div class="carousel-indicators">
                          ${this._urls.map(
                              (_, i) => html`<button
                                  type="button"
                                  class=${i === this._index ? 'active' : ''}
                                  aria-label="Slide ${i + 1}"
                                  @click=${(e) => this.#goTo(e, i)}></button>`,
                          )}
                      </div>
                      <button class="carousel-control prev" type="button" aria-label="Previous" @click=${(e) => this.#go(e, -1)}>
                          <span class="icon" aria-hidden="true"></span>
                      </button>
                      <button class="carousel-control next" type="button" aria-label="Next" @click=${(e) => this.#go(e, 1)}>
                          <span class="icon" aria-hidden="true"></span>
                      </button>
                  `
                : nothing}
        `;
    }

    static styles = css`
        :host {
            display: block;
        }

        .carousel {
            position: relative;
            overflow: hidden;
            aspect-ratio: ${IMAGE_WIDTH} / ${IMAGE_HEIGHT};
            background: var(--uui-color-surface-alt);
        }

        .carousel-inner {
            display: flex;
            height: 100%;
            transition: transform 0.6s ease-in-out;
        }

        .carousel-inner img {
            flex: 0 0 100%;
            width: 100%;
            height: 100%;
            object-fit: cover;
            user-select: none;
        }

        .placeholder {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: var(--uui-size-space-3);
            height: 100%;
            color: var(--uui-color-text-alt);
            font-size: var(--uui-type-small-size);
            text-decoration: none;
        }

        .placeholder uui-icon {
            font-size: 2rem;
        }

        .carousel-control {
            position: absolute;
            top: 0;
            bottom: 0;
            z-index: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 15%;
            padding: 0;
            background: none;
            border: 0;
            cursor: pointer;
            opacity: 0.5;
            transition: opacity 0.15s ease;
        }

        .carousel-control:hover,
        .carousel-control:focus-visible {
            outline: 0;
            opacity: 0.9;
        }

        .carousel-control.prev {
            left: 0;
        }

        .carousel-control.next {
            right: 0;
        }

        .carousel-control .icon {
            display: inline-block;
            width: 2rem;
            height: 2rem;
            background-repeat: no-repeat;
            background-position: 50%;
            background-size: 100% 100%;
        }

        .carousel-control.prev .icon {
            background-image: ${unsafeCSS(PREV_ICON)};
        }

        .carousel-control.next .icon {
            background-image: ${unsafeCSS(NEXT_ICON)};
        }

        .carousel-indicators {
            position: absolute;
            right: 0;
            bottom: 0;
            left: 0;
            z-index: 2;
            display: flex;
            justify-content: center;
            margin: 0 15% 1rem;
        }

        .carousel-indicators button {
            box-sizing: content-box;
            flex: 0 1 auto;
            width: 30px;
            height: 3px;
            padding: 0;
            margin: 0 3px;
            cursor: pointer;
            background-color: #fff;
            background-clip: padding-box;
            border: 0;
            border-top: 10px solid transparent;
            border-bottom: 10px solid transparent;
            opacity: 0.5;
            transition: opacity 0.6s ease;
        }

        .carousel-indicators button.active {
            opacity: 1;
        }

        @media (prefers-reduced-motion: reduce) {
            .carousel-inner,
            .carousel-control,
            .carousel-indicators button {
                transition: none;
            }
        }
    `;
}

customElements.define('umbraco26-carousel-preview', CarouselPreviewElement);

export default CarouselPreviewElement;
