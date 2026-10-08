import { LitElement, css, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { META_ROBOTS_DEFAULT, META_ROBOTS_VALUES } from './meta-robots.values.js';

/**
 * Property Editor UI for the "Meta Robots" data type: a dropdown with ALL and NONE, stored as a plain string.
 * Pages set to NONE are left out of the sitemap. A page without a value, like one created before the property
 * existed, shows and counts as ALL; new pages get ALL from meta-robots.preset.js.
 */
export class MetaRobotsElement extends UmbElementMixin(LitElement) {
    static properties = {
        value: { type: String },
        readonly: { type: Boolean, reflect: true },
    };

    #onChange(event) {
        this.value = event.target.value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    render() {
        const selected = META_ROBOTS_VALUES.includes(this.value) ? this.value : META_ROBOTS_DEFAULT;
        const options = META_ROBOTS_VALUES.map((value) => ({ name: value, value, selected: value === selected }));

        return html`<uui-select
            label="Meta Robots"
            .options=${options}
            .value=${selected}
            ?readonly=${this.readonly}
            @change=${this.#onChange}></uui-select>`;
    }

    static styles = css`
        :host {
            display: block;
        }
    `;
}

customElements.define('umbraco26-meta-robots', MetaRobotsElement);

export default MetaRobotsElement;
