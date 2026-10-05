import { LitElement, css, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';

/**
 * "Contact submissions" dashboard in the Content section. Lists what visitors sent with the
 * contact form block, newest first, 20 per page, from ContactSubmissionsApiController.
 * The API decrypts the email addresses; it only answers users with Content access or admins.
 */

const PAGE_SIZE = 20;
const API_PATH = '/umbraco/contact-submissions/api/v1/submissions';

export class ContactSubmissionsDashboardElement extends UmbElementMixin(LitElement) {
    static properties = {
        _items: { state: true },
        _total: { state: true },
        _page: { state: true },
        _loading: { state: true },
        _error: { state: true },
    };

    #authContext;

    constructor() {
        super();
        this._items = [];
        this._total = 0;
        this._page = 1;
        this._loading = true;
        this._error = undefined;

        this.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
            this.#authContext = authContext;
            this.#load();
        });
    }

    get #totalPages() {
        return Math.max(1, Math.ceil(this._total / PAGE_SIZE));
    }

    async #load() {
        if (!this.#authContext) return;

        this._loading = true;
        this._error = undefined;

        // Base URL, credentials and token as the backoffice itself uses them for the Management API
        const config = this.#authContext.getOpenApiConfiguration();
        const query = new URLSearchParams({ skip: String((this._page - 1) * PAGE_SIZE), take: String(PAGE_SIZE) });

        try {
            const response = await fetch(`${config.base}${API_PATH}?${query}`, {
                credentials: config.credentials,
                headers: {
                    Accept: 'application/json',
                    Authorization: `Bearer ${await config.token()}`,
                },
            });

            if (!response.ok) {
                throw new Error(`${response.status} ${response.statusText}`);
            }

            const data = await response.json();
            this._items = data.items ?? [];
            this._total = data.total ?? 0;
        } catch (error) {
            this._items = [];
            this._error = error instanceof Error ? error.message : String(error);
        } finally {
            this._loading = false;
        }
    }

    #onPageChange(event) {
        const page = event.target.current;
        if (page === this._page) return;

        this._page = page;
        this.#load();
    }

    #formatDate(value) {
        return this.localize.date(value, { dateStyle: 'medium', timeStyle: 'short' });
    }

    render() {
        return html`
            <uui-box headline="Contact submissions">
                <span slot="header-actions" class="count">${this._total} in total</span>
                ${this.#renderBody()}
            </uui-box>
        `;
    }

    #renderBody() {
        if (this._loading && !this._items.length) {
            return html`<div class="center"><uui-loader></uui-loader></div>`;
        }

        if (this._error) {
            return html`<p class="error">Could not load the submissions (${this._error}).</p>`;
        }

        if (!this._items.length) {
            return html`<p class="empty">No one has used the contact form yet.</p>`;
        }

        return html`
            <uui-table aria-label="Contact submissions">
                <uui-table-head>
                    <uui-table-head-cell class="date">Date</uui-table-head-cell>
                    <uui-table-head-cell>Name</uui-table-head-cell>
                    <uui-table-head-cell>Email</uui-table-head-cell>
                    <uui-table-head-cell class="comment">Comment</uui-table-head-cell>
                    <uui-table-head-cell>Page</uui-table-head-cell>
                </uui-table-head>
                ${this._items.map(
                    (item) => html`
                        <uui-table-row>
                            <uui-table-cell class="date">${this.#formatDate(item.createdUtc)}</uui-table-cell>
                            <uui-table-cell>${item.name}</uui-table-cell>
                            <uui-table-cell>
                                ${item.email
                                    ? html`<a href="mailto:${item.email}">${item.email}</a>`
                                    : html`<span class="muted">Can't be decrypted</span>`}
                            </uui-table-cell>
                            <uui-table-cell class="comment">${item.comment}</uui-table-cell>
                            <uui-table-cell>
                                ${item.pageName ?? html`<span class="muted">Deleted page</span>`}
                                ${item.culture ? html`<span class="muted">(${item.culture})</span>` : nothing}
                            </uui-table-cell>
                        </uui-table-row>
                    `,
                )}
            </uui-table>

            ${this.#totalPages > 1
                ? html`<uui-pagination
                      .total=${this.#totalPages}
                      .current=${this._page}
                      @change=${this.#onPageChange}></uui-pagination>`
                : nothing}
        `;
    }

    static styles = css`
        :host {
            display: block;
            padding: var(--uui-size-layout-1);
        }

        .count,
        .muted {
            color: var(--uui-color-text-alt);
        }

        .center {
            display: flex;
            justify-content: center;
            padding: var(--uui-size-space-6);
        }

        .error {
            color: var(--uui-color-danger);
        }

        .date {
            white-space: nowrap;
        }

        .comment {
            min-width: 20rem;
            white-space: pre-wrap;
        }

        uui-pagination {
            display: block;
            margin-top: var(--uui-size-space-5);
        }
    `;
}

customElements.define('umbraco26-contact-submissions-dashboard', ContactSubmissionsDashboardElement);

export default ContactSubmissionsDashboardElement;
