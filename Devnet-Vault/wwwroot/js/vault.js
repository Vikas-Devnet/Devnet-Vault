/**
 * vault.js — Global UI Utilities
 * ─────────────────────────────────────────────────────────
 *  Vault.alert(type, message, duration)  → themed toast notification
 *  Vault.loader.show(text?)             → full-screen overlay loader
 *  Vault.loader.hide()                  → hide loader
 *  Vault.confirm(message, onConfirm)    → themed confirm dialog
 * ─────────────────────────────────────────────────────────
 */

(function (global) {
    'use strict';

    /* ═══════════════════════════════════════════════════
       INJECT STYLES (so vault.js is self-contained)
    ═══════════════════════════════════════════════════ */
    const STYLES = `
    /* ── Toast Container ── */
    #vault-toast-container {
        position: fixed;
        top: 80px;
        right: 1.25rem;
        z-index: 9999;
        display: flex;
        flex-direction: column;
        gap: .6rem;
        pointer-events: none;
        max-width: 360px;
        width: calc(100vw - 2.5rem);
    }

    /* ── Toast ── */
    .vault-toast {
        display: flex;
        align-items: flex-start;
        gap: .75rem;
        padding: .85rem 1rem .85rem 1rem;
        border-radius: .75rem;
        background: #fff;
        border: 1px solid #e2e8f0;
        box-shadow: 0 8px 32px rgba(15,23,42,.13);
        pointer-events: all;
        animation: vt-in .3s cubic-bezier(.34,1.56,.64,1) both;
        position: relative;
        overflow: hidden;
        cursor: default;
        transition: opacity .25s ease, transform .25s ease;
    }
    .vault-toast.vt-hiding {
        animation: vt-out .28s ease forwards;
    }

    /* Accent bar */
    .vault-toast::before {
        content: '';
        position: absolute;
        left: 0; top: 0; bottom: 0;
        width: 4px;
        border-radius: .75rem 0 0 .75rem;
    }

    /* Progress bar */
    .vault-toast::after {
        content: '';
        position: absolute;
        bottom: 0; left: 0;
        height: 2px;
        background: currentColor;
        opacity: .25;
        width: 100%;
        transform-origin: left;
        animation: vt-progress var(--vt-dur, 4s) linear forwards;
    }

    @keyframes vt-progress {
        from { transform: scaleX(1); }
        to   { transform: scaleX(0); }
    }

    /* Type variants */
    .vault-toast.vt-success { color: #15803d; }
    .vault-toast.vt-success::before { background: #22c55e; }
    .vault-toast.vt-success .vt-icon-wrap { background: #f0fdf4; color: #22c55e; }

    .vault-toast.vt-error { color: #dc2626; }
    .vault-toast.vt-error::before { background: #ef4444; }
    .vault-toast.vt-error .vt-icon-wrap { background: #fef2f2; color: #ef4444; }

    .vault-toast.vt-warning { color: #b45309; }
    .vault-toast.vt-warning::before { background: #f59e0b; }
    .vault-toast.vt-warning .vt-icon-wrap { background: #fffbeb; color: #f59e0b; }

    .vault-toast.vt-info { color: #1d4ed8; }
    .vault-toast.vt-info::before { background: #2563eb; }
    .vault-toast.vt-info .vt-icon-wrap { background: #eff6ff; color: #2563eb; }

    /* Icon wrap */
    .vt-icon-wrap {
        width: 34px; height: 34px;
        border-radius: 50%;
        display: flex; align-items: center; justify-content: center;
        font-size: 1rem;
        flex-shrink: 0;
        margin-top: 1px;
    }

    /* Content */
    .vt-content { flex: 1; min-width: 0; }
    .vt-title {
        font-size: .8rem;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: .05em;
        margin-bottom: .15rem;
        line-height: 1;
    }
    .vt-message {
        font-size: .875rem;
        font-weight: 500;
        color: #334155;
        line-height: 1.45;
        word-break: break-word;
    }

    /* Close button */
    .vt-close {
        background: none; border: none; padding: 0;
        width: 22px; height: 22px;
        border-radius: 50%;
        display: flex; align-items: center; justify-content: center;
        color: #94a3b8; font-size: .8rem;
        cursor: pointer; flex-shrink: 0; margin-top: 1px;
        transition: background .15s, color .15s;
    }
    .vt-close:hover { background: #f1f5f9; color: #475569; }

    @keyframes vt-in {
        from { opacity: 0; transform: translateX(24px) scale(.95); }
        to   { opacity: 1; transform: translateX(0) scale(1); }
    }
    @keyframes vt-out {
        from { opacity: 1; transform: translateX(0) scale(1); max-height: 200px; margin-bottom: 0; }
        to   { opacity: 0; transform: translateX(24px) scale(.95); max-height: 0; margin-bottom: -0.6rem; }
    }

    /* ── Full-screen Loader ── */
    #vault-loader {
        position: fixed;
        inset: 0;
        z-index: 99999;
        background: rgba(15, 23, 42, 0.55);
        backdrop-filter: blur(4px);
        -webkit-backdrop-filter: blur(4px);
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        gap: 1.25rem;
        opacity: 0;
        pointer-events: none;
        transition: opacity .25s ease;
    }
    #vault-loader.vl-visible {
        opacity: 1;
        pointer-events: all;
    }
    .vl-spinner {
        width: 52px; height: 52px;
        border-radius: 50%;
        border: 3px solid rgba(255,255,255,.15);
        border-top-color: #2563eb;
        border-right-color: #6366f1;
        animation: vl-spin .7s linear infinite;
        position: relative;
    }
    .vl-spinner::after {
        content: '';
        position: absolute;
        inset: 5px;
        border-radius: 50%;
        border: 2px solid transparent;
        border-bottom-color: #818cf8;
        animation: vl-spin .45s linear infinite reverse;
    }
    @keyframes vl-spin { to { transform: rotate(360deg); } }

    .vl-text {
        color: rgba(255,255,255,.85);
        font-size: .9rem;
        font-weight: 500;
        letter-spacing: .02em;
        text-align: center;
    }

    /* ── Confirm Dialog ── */
    #vault-confirm-overlay {
        position: fixed;
        inset: 0;
        z-index: 99998;
        background: rgba(15,23,42,.5);
        backdrop-filter: blur(3px);
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 1rem;
        opacity: 0;
        pointer-events: none;
        transition: opacity .2s ease;
    }
    #vault-confirm-overlay.vc-visible {
        opacity: 1;
        pointer-events: all;
    }
    #vault-confirm-box {
        background: #fff;
        border-radius: 1.25rem;
        padding: 2rem;
        max-width: 400px;
        width: 100%;
        box-shadow: 0 20px 60px rgba(15,23,42,.2);
        transform: translateY(16px) scale(.96);
        transition: transform .25s cubic-bezier(.34,1.56,.64,1);
    }
    #vault-confirm-overlay.vc-visible #vault-confirm-box {
        transform: translateY(0) scale(1);
    }
    .vc-icon {
        width: 52px; height: 52px;
        border-radius: 50%;
        background: #fef2f2;
        color: #ef4444;
        display: flex; align-items: center; justify-content: center;
        font-size: 1.4rem;
        margin-bottom: 1rem;
    }
    .vc-title {
        font-size: 1.1rem;
        font-weight: 700;
        color: #0f172a;
        margin-bottom: .4rem;
    }
    .vc-message {
        font-size: .9rem;
        color: #475569;
        line-height: 1.6;
        margin-bottom: 1.5rem;
    }
    .vc-actions { display: flex; gap: .75rem; }
    .vc-btn-cancel {
        flex: 1; padding: .65rem;
        border: 1.5px solid #e2e8f0;
        border-radius: 9999px;
        background: #fff;
        color: #475569;
        font-weight: 600;
        font-size: .875rem;
        cursor: pointer;
        transition: background .15s, border-color .15s;
    }
    .vc-btn-cancel:hover { background: #f8fafc; border-color: #cbd5e1; }
    .vc-btn-confirm {
        flex: 1; padding: .65rem;
        border: none;
        border-radius: 9999px;
        background: #ef4444;
        color: #fff;
        font-weight: 600;
        font-size: .875rem;
        cursor: pointer;
        transition: background .15s, box-shadow .15s;
    }
    .vc-btn-confirm:hover { background: #dc2626; box-shadow: 0 4px 14px rgba(239,68,68,.35); }
    `;

    function injectStyles() {
        if (document.getElementById('vault-js-styles')) return;
        const style = document.createElement('style');
        style.id = 'vault-js-styles';
        style.textContent = STYLES;
        document.head.appendChild(style);
    }

    /* ═══════════════════════════════════════════════════
       TOAST CONTAINER
    ═══════════════════════════════════════════════════ */
    function getContainer() {
        let c = document.getElementById('vault-toast-container');
        if (!c) {
            c = document.createElement('div');
            c.id = 'vault-toast-container';
            document.body.appendChild(c);
        }
        return c;
    }

    /* ═══════════════════════════════════════════════════
       TOAST CONFIG
    ═══════════════════════════════════════════════════ */
    const TOAST_CONFIG = {
        success: { icon: 'bi-check-circle-fill', label: 'Success' },
        error: { icon: 'bi-x-circle-fill', label: 'Error' },
        warning: { icon: 'bi-exclamation-triangle-fill', label: 'Warning' },
        info: { icon: 'bi-info-circle-fill', label: 'Info' }
    };

    /**
     * Show a themed toast notification.
     * @param {'success'|'error'|'warning'|'info'} type
     * @param {string} message
     * @param {number} duration  ms before auto-dismiss (0 = manual only)
     */
    function alert(type, message, duration) {
        if (typeof duration === 'undefined') duration = 4000;
        injectStyles();

        const cfg = TOAST_CONFIG[type] || TOAST_CONFIG.info;
        const toast = document.createElement('div');
        toast.className = `vault-toast vt-${type}`;
        if (duration > 0) {
            toast.style.setProperty('--vt-dur', (duration / 1000) + 's');
        } else {
            toast.style.setProperty('--vt-dur', '0s');
            toast.style.setProperty('animation', 'none', '');
        }

        toast.innerHTML = `
            <div class="vt-icon-wrap">
                <i class="bi ${cfg.icon}"></i>
            </div>
            <div class="vt-content">
                <div class="vt-title">${cfg.label}</div>
                <div class="vt-message">${message}</div>
            </div>
            <button class="vt-close" aria-label="Dismiss">
                <i class="bi bi-x"></i>
            </button>
        `;

        const container = getContainer();
        container.appendChild(toast);

        // Close on button click
        toast.querySelector('.vt-close').addEventListener('click', () => dismiss(toast));

        // Auto-dismiss
        let timer = null;
        if (duration > 0) {
            timer = setTimeout(() => dismiss(toast), duration);
        }

        // Pause timer on hover
        toast.addEventListener('mouseenter', () => {
            if (timer) clearTimeout(timer);
            toast.style.setProperty('animation-play-state', 'paused');
        });
        toast.addEventListener('mouseleave', () => {
            if (duration > 0) {
                timer = setTimeout(() => dismiss(toast), 1500);
            }
        });

        return toast;
    }

    function dismiss(toast) {
        if (!toast || !toast.parentNode) return;
        toast.classList.add('vt-hiding');
        toast.addEventListener('animationend', () => toast.remove(), { once: true });
        // Fallback
        setTimeout(() => toast.remove(), 400);
    }

    /* ═══════════════════════════════════════════════════
       LOADER
    ═══════════════════════════════════════════════════ */
    let _loaderEl = null;

    function getLoader() {
        if (!_loaderEl) {
            injectStyles();
            _loaderEl = document.createElement('div');
            _loaderEl.id = 'vault-loader';
            _loaderEl.innerHTML = `
                <div class="vl-spinner"></div>
                <div class="vl-text" id="vault-loader-text">Please wait…</div>
            `;
            document.body.appendChild(_loaderEl);
        }
        return _loaderEl;
    }

    const loader = {
        /**
         * Show the full-screen loader overlay.
         * @param {string} [text='Please wait…']
         */
        show(text) {
            const el = getLoader();
            el.querySelector('#vault-loader-text').textContent = text || 'Please wait…';
            // Force reflow then add class for CSS transition
            el.offsetHeight;
            el.classList.add('vl-visible');
        },

        /** Hide the full-screen loader overlay. */
        hide() {
            const el = getLoader();
            el.classList.remove('vl-visible');
        }
    };

    /* ═══════════════════════════════════════════════════
       CONFIRM DIALOG
    ═══════════════════════════════════════════════════ */
    let _confirmOverlay = null;

    function getConfirmOverlay() {
        if (!_confirmOverlay) {
            injectStyles();
            _confirmOverlay = document.createElement('div');
            _confirmOverlay.id = 'vault-confirm-overlay';
            _confirmOverlay.innerHTML = `
                <div id="vault-confirm-box">
                    <div class="vc-icon"><i class="bi bi-exclamation-triangle-fill"></i></div>
                    <div class="vc-title" id="vc-title">Are you sure?</div>
                    <div class="vc-message" id="vc-message">This action cannot be undone.</div>
                    <div class="vc-actions">
                        <button class="vc-btn-cancel" id="vc-cancel">Cancel</button>
                        <button class="vc-btn-confirm" id="vc-confirm">Confirm</button>
                    </div>
                </div>
            `;
            document.body.appendChild(_confirmOverlay);
        }
        return _confirmOverlay;
    }

    /**
     * Show a themed confirm dialog.
     * @param {string|object} options  — string message OR { title, message, confirmText, cancelText }
     * @param {Function} onConfirm     — called when user clicks Confirm
     * @param {Function} [onCancel]    — called when user clicks Cancel
     */
    function confirm(options, onConfirm, onCancel) {
        const overlay = getConfirmOverlay();

        const title = typeof options === 'string' ? 'Are you sure?' : (options.title || 'Are you sure?');
        const message = typeof options === 'string' ? options : (options.message || '');
        const confirmText = typeof options === 'object' ? (options.confirmText || 'Confirm') : 'Confirm';
        const cancelText = typeof options === 'object' ? (options.cancelText || 'Cancel') : 'Cancel';

        overlay.querySelector('#vc-title').textContent = title;
        overlay.querySelector('#vc-message').textContent = message;
        overlay.querySelector('#vc-confirm').textContent = confirmText;
        overlay.querySelector('#vc-cancel').textContent = cancelText;

        overlay.offsetHeight;
        overlay.classList.add('vc-visible');

        function close() {
            overlay.classList.remove('vc-visible');
            // Clean up listeners
            overlay.querySelector('#vc-confirm').onclick = null;
            overlay.querySelector('#vc-cancel').onclick = null;
            overlay.onclick = null;
        }

        overlay.querySelector('#vc-confirm').onclick = () => {
            close();
            if (typeof onConfirm === 'function') onConfirm();
        };

        overlay.querySelector('#vc-cancel').onclick = () => {
            close();
            if (typeof onCancel === 'function') onCancel();
        };

        // Click outside to cancel
        overlay.onclick = (e) => {
            if (e.target === overlay) {
                close();
                if (typeof onCancel === 'function') onCancel();
            }
        };
    }

    /* ═══════════════════════════════════════════════════
       NAVBAR SCROLL SHADOW  (auto-init on DOMReady)
    ═══════════════════════════════════════════════════ */
    function initNavbarShadow() {
        const navbar = document.querySelector('.vault-navbar');
        if (!navbar) return;
        function toggle() {
            navbar.classList.toggle('shadow', window.scrollY > 10);
        }
        window.addEventListener('scroll', toggle, { passive: true });
        toggle();
    }

    /* ═══════════════════════════════════════════════════
       SCROLL REVEAL  (auto-init on DOMReady)
    ═══════════════════════════════════════════════════ */
    function initScrollReveal() {
        if (!('IntersectionObserver' in window)) return;

        const io = new IntersectionObserver((entries) => {
            entries.forEach(e => {
                if (e.isIntersecting) {
                    e.target.style.opacity = '1';
                    e.target.style.transform = 'translateY(0)';
                    io.unobserve(e.target);
                }
            });
        }, { threshold: 0.12 });

        document.querySelectorAll('.vault-animate').forEach(el => {
            if (el.style.opacity === '1') return;
            const delay = el.classList.contains('vault-delay-4') ? '0.46s'
                : el.classList.contains('vault-delay-3') ? '0.34s'
                    : el.classList.contains('vault-delay-2') ? '0.22s'
                        : el.classList.contains('vault-delay-1') ? '0.10s'
                            : '0s';
            el.style.cssText += `opacity:0;transform:translateY(28px);
                transition:opacity .55s ease ${delay},transform .55s ease ${delay};`;
            io.observe(el);
        });
    }

    /* ═══════════════════════════════════════════════════
       AUTO-INIT
    ═══════════════════════════════════════════════════ */
    function init() {
        injectStyles();
        initNavbarShadow();
        initScrollReveal();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    /* ═══════════════════════════════════════════════════
       PUBLIC API
    ═══════════════════════════════════════════════════ */
    global.Vault = {
        /**
         * Show a themed toast.
         * Vault.alert('success', 'File uploaded!', 4000)
         * Vault.alert('error',   'Something went wrong.')
         * Vault.alert('warning', 'Storage almost full!', 6000)
         * Vault.alert('info',    'New version available.', 0)  // 0 = no auto-dismiss
         */
        alert,

        /** Vault.loader.show('Uploading…') / Vault.loader.hide() */
        loader,

        /**
         * Vault.confirm('Delete this file?', () => doDelete())
         * Vault.confirm({ title:'Delete?', message:'Permanent.', confirmText:'Yes, delete' }, onConfirm, onCancel)
         */
        confirm
    };

})(window);
