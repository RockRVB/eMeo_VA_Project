class Header extends HTMLElement {
    constructor() {
        super();
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    render() {
        this.innerHTML = `
      <header class="stm-header-two">
            <div class="header-logo">
                <img src="../../../images/VA/logo.svg" alt="Logo" class="logo-img">
            </div>
            <div class="header-title">
                <h2 ids="ids_VAB_header_text_not_auth">Kính chào quý khách !</h2>
            </div>
            <div class="header-lang">
                <button class="switch-lang-btn lang-item active">VI</button>
                <span class="lang-divider">|</span>
                <button class="switch-lang-btn lang-item">EN</button>
            </div>
        </header>
    `;
    }
}

class OnlyLogoHeader extends HTMLElement {
    constructor() {
        super();
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    render() {
        this.innerHTML = `
      <header class="stm-header transfer-header">
            <div class="header-logo">
                <img src="../../../images/VA/logo.svg" alt="Logo" class="logo-img">
            </div>
        </header>
    `;
    }
}

class Footer extends HTMLElement {
    constructor() {
        super();
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    render() {
        this.innerHTML = `
        <footer class="stm-footer" style="padding-bottom: 32px;">
        <div ids="ids_VAB_footer_hotline">1900 555 590 - 02836 222 590</div>
       <div ids="ids_VAB_footer_website">www.vietabank.com.vn</div>
        </footer>
    `;
    }
}


class FooterWithContinueBtn extends HTMLElement {
    constructor() {
        super();
        this._continueBtnClick = null;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    set onContinueBtnClick(handler) {
        this._continueBtnClick = handler;
        this.updateClickEvent();
    }

    updateClickEvent() {
        const btn = this.querySelector('#continue-trans-btn');
        if (btn && this._continueBtnClick) {
            btn.onclick = this._continueBtnClick;
        }
    }

    render() {
        this.innerHTML = `
        <footer class="stm-footer-continue-btn footer-actions">
            <div class="footer-spacer"></div>
            <div class="footer-contact" style="flex-direction: column;">
                <div class="contact-text" ids="ids_VAB_footer_hotline">1900 555 590 - 02836 222 590</div>
                <div class="contact-text" ids="ids_VAB_footer_website">www.vietabank.com.vn</div>
            </div>
            <div class="footer-btn-wrap">
                <button class="btn-footer-primary" tag="onContinueBtn" id="continue-trans-btn">
                    <span ids="ids_VAB_continue_btn">Tiếp tục</span>
                    <img src="../../../images/VA/chevron-right.svg">
                </button>
            </div>
        </footer>
    `;
    }
}


class FooterPreviousScreenBtn extends HTMLElement {
    constructor() {
        super();
        this._previousBtnClick = null;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    set onPreviousBtnClick(handler) {
        this._previousBtnClick = handler;
        this.updateClickEvent();
    }

    updateClickEvent() {
        const btn = this.querySelector('#to-previous-page');
        if (btn && this._previousBtnClick) {
            btn.onclick = this._previousBtnClick;
        }
    }

    render() {
        this.innerHTML = `
        <footer class="stm-footer-previous-btn">
            <div class="footer-previous-btn-wrap">
                <button class="btn-footer-secondary" tag="onPreviousBtn" id="to-previous-page">
                    <img src="../../../images/VA/chevron-left.svg">
                    <span ids="ids_VAB_continue_btn">Quay lại</span>
                </button>
            </div>
            <div class="footer-contact" style="flex-direction: column; gap: 8px;">
                <div class="contact-text" ids="ids_VAB_footer_hotline">1900 555 590 - 02836 222 590</div>
                <div class="contact-text" ids="ids_VAB_footer_website">www.vietabank.com.vn</div>
            </div>
        </footer>
    `;
    }
}

class AuthTaskHeader extends HTMLElement {
    constructor() {
        super();
        this.timer = null;
        this.countdownValue = 120;
        this._cancelTransOnClick = null;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
        this.startCountdown();
    }

    disconnectedCallback() {
        if (this.timer) {
            clearInterval(this.timer);
        }
    }

    set onCancelTransBtnClick(handler) {
        this._cancelTransOnClick = handler;
        this.updateClickEvent();
    }

    updateClickEvent() {
        const btn = this.querySelector('#header-cancel-trans-btn');
        if (btn && this._cancelTransOnClick) {
            btn.onclick = this._cancelTransOnClick;
        }
    }

    startCountdown() {
        const countdownElement = this.querySelector('.header-countdown');
        if (!countdownElement) return;

        this.timer = setInterval(() => {
            this.countdownValue--;

            if (this.countdownValue >= 0) {
                // Cập nhật nội dung thẻ span
                countdownElement.textContent = `Giao dịch kết thúc sau ${this.countdownValue} s`;
            } else {
                // Dừng đếm ngược khi về 0
                clearInterval(this.timer);
            }
        }, 1000);
    }

    render() {
        this.innerHTML = `
      <header class="stm-header transfer-header">
            <div class="header-logo-group">
                <img src="../../../images/VA/logo.svg" alt="Logo" class="logo-img">
            </div>
            
            <div class="header-actions">
                <span class="header-countdown" ids="ids_VAB_end_transaction_in_seconds">Giao dịch kết thúc sau ${this.countdownValue} s</span>
                <button class="btn-cancel-transaction" id="header-cancel-trans-btn" tag="onCancelTrans">
                    <img src="../../../images/VA/close-circle.svg">
                    <span ids="ids_VAB_end_transaction">Hủy giao dịch</span>
                </button>
            </div>
        </header>
    `;
    }
}


class AuthHeader extends HTMLElement {
    constructor() {
        super();
        this.timer = null;
        this.countdownValue = 120;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
        this.startCountdown();
    }

    disconnectedCallback() {
        if (this.timer) {
            clearInterval(this.timer);
        }
    }

    startCountdown() {
        const countdownElement = this.querySelector('.header-countdown');
        if (!countdownElement) return;

        this.timer = setInterval(() => {
            this.countdownValue--;

            if (this.countdownValue >= 0) {
                // Cập nhật nội dung thẻ span
                countdownElement.textContent = `Giao dịch kết thúc sau ${this.countdownValue} s`;
            } else {
                // Dừng đếm ngược khi về 0
                clearInterval(this.timer);
            }
        }, 1000);
    }

    render() {
        this.innerHTML = `
      <header class="stm-header-two">
            <div class="header-logo">
                <img src="../../../images/VA/logo.svg" alt="Logo" class="logo-img">
            </div>
            
            <div class="header-title">
                <h2 ids="ids_VAB_hello_customer" id="hello-customer-name">Xin chào NGUYEN VAN TEO</h2>
            </div>
            
            <div class="header-actions">
                <span class="header-countdown" ids="ids_VAB_end_transaction_in_seconds">Giao dịch kết thúc sau ${this.countdownValue} s</span>
                <button class="btn-cancel-transaction" tag="onCloseAll">
                    <img src="../../../images/VA/close-circle.svg">
                    <span ids="ids_VAB_auth_header_close_btn">Đóng</span>
                </button>
            </div>
        </header>
    `;
    }
}


class AuthHeaderNoCusName extends HTMLElement {
    constructor() {
        super();
        this.timer = null;
        this.countdownValue = 120;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
        this.startCountdown();
    }

    disconnectedCallback() {
        if (this.timer) {
            clearInterval(this.timer);
        }
    }

    startCountdown() {
        const countdownElement = this.querySelector('.header-countdown');
        if (!countdownElement) return;

        this.timer = setInterval(() => {
            this.countdownValue--;

            if (this.countdownValue >= 0) {
                // Cập nhật nội dung thẻ span
                countdownElement.textContent = `Giao dịch kết thúc sau ${this.countdownValue} s`;
            } else {
                // Dừng đếm ngược khi về 0
                clearInterval(this.timer);
            }
        }, 1000);
    }

    render() {
        this.innerHTML = `
      <header class="stm-header transfer-header">
            <div class="header-logo">
                <img src="../../../images/VA/logo.svg" alt="Logo" class="logo-img">
            </div>
            
            <div class="header-actions">
                <span class="header-countdown" ids="ids_VAB_end_transaction_in_seconds">Giao dịch kết thúc sau ${this.countdownValue} s</span>
                <button class="btn-cancel-transaction" tag="onCloseAll">
                    <img src="../../../images/VA/close-circle.svg">
                    <span ids="ids_VAB_auth_header_close_btn">Đóng</span>
                </button>
            </div>
        </header>
    `;
    }
}


class AuthHeaderNoCusName_BackToHome extends HTMLElement {
    constructor() {
        super();
        this.timer = null;
        this.countdownValue = 120;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
        this.startCountdown();
    }

    disconnectedCallback() {
        if (this.timer) {
            clearInterval(this.timer);
        }
    }

    startCountdown() {
        const countdownElement = this.querySelector('.header-countdown');
        if (!countdownElement) return;

        this.timer = setInterval(() => {
            this.countdownValue--;

            if (this.countdownValue >= 0) {
                // Cập nhật nội dung thẻ span
                countdownElement.textContent = `Tự động về trang chủ sau ${this.countdownValue} s`;
            } else {
                // Dừng đếm ngược khi về 0
                clearInterval(this.timer);
            }
        }, 1000);
    }

    render() {
        this.innerHTML = `
      <header class="stm-header transfer-header">
            <div class="header-logo">
                <img src="../../../images/VA/logo.svg" alt="Logo" class="logo-img">
            </div>
            
            <div class="header-actions">
                <span class="header-countdown" ids="ids_VAB_end_transaction_in_seconds">Tự động về trang chủ sau ${this.countdownValue} s</span>
                <button class="btn-cancel-transaction" tag="onCloseAll">
                    <img src="../../../images/VA/close-circle.svg">
                    <span ids="ids_VAB_auth_header_close_btn">Đóng</span>
                </button>
            </div>
        </header>
    `;
    }
}

class StmCard extends HTMLElement {
    constructor() {
        super();
    }

    static get observedAttributes() {
        return ['icon-url', 'title', 'ids'];
    }

    connectedCallback() {
        this.render();
    }

    attributeChangedCallback(name, oldValue, newValue) {
        // Nếu component đã render rồi thì mới render lại để cập nhật giao diện
        if (this.innerHTML) {
            this.render();
        }
    }

    // 4. Hàm render dữ liệu ra giao diện
    render() {
        // Lấy giá trị của thuộc tính, nếu không có thì dùng giá trị mặc định
        const iconUrl = this.getAttribute('icon-url') || '';
        const title = this.getAttribute('title') || 'default-title';
        const ids = this.getAttribute('ids') || '';

        // Gắn cấu trúc HTML vào component
        this.innerHTML = `
                <div class="card-icon-wrap">
                    <img src="${iconUrl}" alt="${title}">
                </div>
                <div ids="${ids}" class="card-text">${title}</div>
        `;
    }
}


class CancelTransOrNotDialog extends HTMLElement {
    constructor() {
        super();
        this.timer = null;
        this.countdownValue = 60;
        this._isOpen = false;
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    disconnectedCallback() {
        if (this.timer) {
            clearInterval(this.timer);
        }
    }

    stopTimer() {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }

    set openDialog(isOpen){
        this._isOpen = isOpen;
        if (isOpen) {
            this.countdownValue = 60;
            this.render();
            this.startCountdown();
        } else {
            this.stopTimer();
            this.render();
        }
    }

    startCountdown() {
        this.stopTimer();

        if (!this._isOpen) return;

        const countdownElement = this.querySelector('#count-down-back-home');
        if (!countdownElement) return;

        this.timer = setInterval(() => {

            if (this.countdownValue <= 0) {
                this.stopTimer();
                return;
            }

            this.countdownValue--;
            countdownElement.textContent = `Tự động về màn hình Trang chủ trong ${this.countdownValue} s`;
        }, 1000);
    }

    goToHome(){
        window.location.href = '../home/TC-04.html';
    }

    render() {
        this.innerHTML = `
      <div>
          <main class="stm-dialog-container" style="display: ${this._isOpen === true ? 'flex' : 'none'}">
                <div class="stm-dialog">
                    <div class="dialog-icon-wrap">
                        <img src="../../../images/VA/caution-circle.svg">
                    </div>
                    
                    <h2 class="dialog-title" ids="ids_VAB_cancel_trans_question">Bạn có chắc chắn muốn hủy giao dịch này không?</h2>
                    
                    <div class="dialog-actions-container">
                        <div class="dialog-actions">
                            <button class="btn-dialog-primary" tag="cancelAndBackHome" id="cancel-and-back-home-btn">
                                <span ids="ids_VAB_cancel_trans">Hủy giao dịch</span>
                            </button>
                            
                            <button class="btn-dialog-secondary" tag="continueTask" id="continue-task-btn">
                                <span ids="ids_VAB_continue_trans">Tiếp tục giao dịch</span>
                            </button>
                        </div>
                        
                        <p class="dialog-helper-text" id="count-down-back-home" ids="ids_VAB_return_home_after_seconds">Tự động về màn hình Trang chủ trong ${this.countdownValue} s</p>
                    </div>
                </div>
            </main>
      </div>
    `;

        const btnCancel = this.querySelector('#cancel-and-back-home-btn');
        const btnContinue = this.querySelector('#continue-task-btn');
        if (btnCancel) {
            btnCancel.addEventListener('click', () => this.goToHome());
        }
        if (btnContinue) {
            btnContinue.addEventListener('click', () => {
                this._isOpen = false;
                this.render()
            });
        }
    }
}


class ServiceNotAvailableDialog extends HTMLElement {
    constructor() {
        super();
        // this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
    }

    render() {
        this.innerHTML = `
      <main class="stm-dialog-container">
            <div class="stm-dialog">
                <div class="dialog-icon-wrap">
                    <img src="../../../images/VA/caution-circle.svg">
                </div>
                
                <h2 class="dialog-title" ids="ids_VAB_service_not_avail">Dịch vụ tạm thời không khả dụng</h2>
                
                <div class="dialog-actions-container">
                    <div class="dialog-actions">
                        <button class="btn-dialog-primary" tag="backToHome">
                            <span ids="ids_VAB_return_home_btn">Quay về trang chủ</span>
                        </button>
                    </div>
                    
                    <p class="dialog-helper-text" ids="ids_VAB_return_home_after_seconds">Tự động về màn hình Trang chủ trong 60 s</p>
                </div>
            </div>
        </main>
    `;
    }
}


class OneButtonDialog extends HTMLElement {
    constructor() {
        super();
        this._title = "";
        this._subTitle = "";
        this._buttonTitle = "";
        this._onClickHandler = null;
        this._idsBtn = "";
        this._idsTitle = "";
        this._idsSubTitle = "";
        // this.attachShadow({ mode: 'open' });
    }

    set onPrimaryClick(handler) {
        this._onClickHandler = handler;
        this.updateClickEvent();
    }

    set titleText(text) {
        this._title = text;
        this.render();
    }

    set subTitle(text) {
        this._subTitle = text;
        this.render();
    }

    set subBtnTitle(text) {
        this._buttonTitle = text;
        this.render();
    }

    set idsBtn(text) {
        this._idsBtn = text;
        this.render();
    }

    set idsTitle(text) {
        this._idsTitle = text;
        this.render();
    }

    set idsSubTitle(text) {
        this._idsSubTitle = text;
        this.render();
    }

    connectedCallback() {
        this.render();
    }

    updateClickEvent() {
        const btn = this.querySelector('#one-btn');
        if (btn && this._onClickHandler) {
            btn.onclick = this._onClickHandler;
        }
    }

    render() {

        this.innerHTML = `
      <main class="stm-dialog-container">
            <div class="stm-dialog">
                <div class="dialog-icon-wrap">
                    <img src="../../../images/VA/caution-circle.svg">
                </div>
                
                <h2 class="dialog-title" ids="${this._idsTitle}">${this._title}</h2>
                
                <div class="dialog-actions-container">
                    <div class="dialog-actions">
                        <button class="btn-dialog-primary" id="one-btn">
                            <span ids="${this._idsBtn}">${this._buttonTitle}</span>
                        </button>
                    </div>
                    
                    <p class="dialog-helper-text" ids="${this._idsSubTitle}">${this._subTitle}</p>
                </div>
            </div>
        </main>
    `;

        this.updateClickEvent();
    }
}


class YesOrNoDialog extends HTMLElement {
    constructor() {
        super();
        this._title = "";
        this._subTitle = "";
        this._yesBtnTitle = "";
        this._noBtnTitle = "";
        this._yesOnClick = null;
        this._noOnClick = null;
        this._idsTitle = "";
        this._idsSubTitle = "";
        this._idsYesBtnTitle = "";
        this._idsNoBtnTitle = "";
        // this.attachShadow({ mode: 'open' });
    }

    set onYesClick(handler) {
        this._yesOnClick = handler;
        this.updateYesBtnClickEvent();
    }

    set onNoClick(handler) {
        this._noOnClick = handler;
        this.updateNoBtnClickEvent();
    }

    set titleText(text) {
        this._title = text;
        this.render();
    }

    set subTitle(text) {
        this._subTitle = text;
        this.render();
    }

    set subYesBtnTitle(text) {
        this._yesBtnTitle = text;
        this.render();
    }

    set subNoBtnTitle(text) {
        this._noBtnTitle = text;
        this.render();
    }

    set idsTitle(text) {
        this._idsTitle = text;
        this.render();
    }

    set idsSubTitle(text) {
        this._idsSubTitle = text;
        this.render();
    }

    set idsYesBtnTitle(text) {
        this._idsYesBtnTitle = text;
        this.render();
    }

    set idsNoBtnTitle(text) {
        this._idsNoBtnTitle = text;
        this.render();
    }

    connectedCallback() {
        this.render();
    }

    updateYesBtnClickEvent() {
        const btn = this.querySelector('#btn-yes');
        if (btn && this._yesOnClick) {
            btn.onclick = this._yesOnClick;
        }
    }

    updateNoBtnClickEvent() {
        const btn = this.querySelector('#btn-no');
        if (btn && this._noOnClick) {
            btn.onclick = this._noOnClick;
        }
    }

    render() {

        this.innerHTML = `
      <main class="stm-dialog-container">
            <div class="stm-dialog">
                <div class="dialog-icon-wrap">
                    <img src="../../../images/VA/caution-circle.svg">
                </div>
                
                <h2 class="dialog-title" ids="${this._idsTitle}">${this._title}</h2>
                
                <div class="dialog-actions-container">
                    <div class="dialog-actions">
                        <button class="btn-dialog-primary" id="btn-yes">
                            <span ids="${this._idsYesBtnTitle}">${this._yesBtnTitle}</span>
                        </button>
                        
                        <button class="btn-dialog-secondary" id="btn-no">
                            <span ids="${this._idsNoBtnTitle}">${this._noBtnTitle}</span>
                        </button>
                    </div>
                    
                    <p class="dialog-helper-text" ids="${this._idsSubTitle}">${this._subTitle}</p>
                </div>
            </div>
        </main>
    `;

        this.updateYesBtnClickEvent();
        this.updateNoBtnClickEvent();
    }
}


class StmProgressBar extends HTMLElement {
    constructor() {
        super();
    }

    static get observedAttributes() {
        return ['steps', 'current-step'];
    }

    connectedCallback() {
        this.render();
    }

    attributeChangedCallback() {
        this.render();
    }

    render() {
        // Lấy dữ liệu từ attribute
        const stepsData = JSON.parse(this.getAttribute('steps') || '[]');
        const currentStep = parseInt(this.getAttribute('current-step') || '1');

        let htmlContent = '<div class="stm-progress-container">';

        stepsData.forEach((step, index) => {
            const stepNumber = index + 1;
            let stepClass = 'step-waiting';
            let iconContent = `<span>${stepNumber}</span>`;

            // Xác định trạng thái của Step
            if (stepNumber < currentStep) {
                stepClass = 'step-done';
                iconContent = `<img src="../../../images/VA/step-done.svg" alt="Done">`;
            } else if (stepNumber === currentStep) {
                stepClass = 'step-active';
            }

            // Thêm HTML cho từng Step
            htmlContent += `
                <div class="stm-step ${stepClass}">
                    <div class="step-icon">${iconContent}</div>
                    <div class="step-label" ids="${step.ids}">${step.label}</div>
                </div>
            `;

            // Thêm vạch nối (Line) nếu không phải step cuối cùng
            if (index < stepsData.length - 1) {
                const lineClass = stepNumber < currentStep ? 'step-connector active' : 'step-connector';
                htmlContent += `<div class="${lineClass}"><div class="step-connector-dot"></div></div>`;
            }
        });

        htmlContent += '</div>';
        this.innerHTML = htmlContent;
    }
}


class StmList extends HTMLElement {
    constructor() {
        super();
    }

    static get observedAttributes() {
        return ['accounts', 'active-id', 'headers'];
    }

    connectedCallback() {
        this.render();
    }

    attributeChangedCallback() {
        this.render();
    }

    getSelectedAccount() {
        const accounts = JSON.parse(this.getAttribute('accounts') || '[]');
        const activeId = this.getAttribute('active-id');
        return accounts.find(acc => acc.id === activeId) || null;
    }

    render() {
        const accounts = JSON.parse(this.getAttribute('accounts') || '[]');
        const headers = JSON.parse(this.getAttribute('headers') || '[]');
        const activeId = this.getAttribute('active-id');

        // Render phần Header
        let headerHtml = `<div class="account-list-headers">`;
        headers.forEach(header => {
            headerHtml += `<span class="header-col" ids="${header.ids}">${header.label}</span>`;
        });
        headerHtml += `</div>`;

        // Duyệt qua danh sách tài khoản để render từng dòng (Row)
        let listHtml = `<div class="stm-account-cards-list">`;
        accounts.forEach(acc => {
            const isActive = acc.id === activeId;
            const cardClass = isActive ? 'stm-account-card active' : 'stm-account-card';
            const iconSrc = isActive ? '../../../images/VA/radio-on.svg' : '../../../images/VA/radio-off.svg';

            listHtml += `
                <div class="${cardClass}" id="account-card-list">
                    <div class="account-radio-icon">
                        <img src="${iconSrc}" alt="${isActive ? 'Selected' : 'Unselected'}">
                    </div>
                    <div class="account-details">
                        <span class="account-number">${acc.accountNumber}</span>
                        <span class="account-balance">${acc.balance} ${acc.currency || 'VND'}</span>
                    </div>
                </div>
            `;
        });

        listHtml += `</div>`;
        this.innerHTML = headerHtml + listHtml;

        const cards = this.querySelectorAll('#account-card-list');
        cards.forEach((card, index) => {
            card.onclick = () => {
                const acc = accounts[index]; // Lấy đúng object dựa trên index
                this.onAccountSelect(acc.id, acc);
            };
        });
    }

    // Hàm xử lý khi click chọn tài khoản
    onAccountSelect(id, account) {
        // Cập nhật attribute để component tự render lại
        this.setAttribute('active-id', id);

        // Gửi một sự kiện (Event) ra ngoài để trang chính xử lý nếu cần
        this.dispatchEvent(new CustomEvent('change', { detail: { account: account } }));
    }
}


class CustomTable extends HTMLElement {
    constructor() {
        super();
    }

    static get observedAttributes() {
        return ['data', 'active-id', 'headers'];
    }

    connectedCallback() {
        this.render();
    }

    attributeChangedCallback() {
        this.render();
    }

    render() {
        const dataList = JSON.parse(this.getAttribute('data') || '[]');
        const headers = JSON.parse(this.getAttribute('headers') || '[]');

        let headerHtml = `<div class="statement-card"><div class="statement-table statement-header"><div class="statement-row">`;
        headers.forEach(h => {
            headerHtml += `<span class="statement-col">${h.label}</span>`;
        });
        headerHtml += `</div></div>`;

        // 2. Render List Data (Tự động ánh xạ)
        let listHtml = `<div class="statement-table statement-data">`;
        dataList.forEach(obj => {
            listHtml += `<div class="statement-row">`;

            // Duyệt qua danh sách header để lấy dữ liệu tương ứng từ obj
            headers.forEach(h => {
                const cellValue = obj[h.field] || ''; // Lấy giá trị theo key động
                listHtml += `<span class="statement-col">${cellValue}</span>`;
            });

            listHtml += `</div>`;
        });
        listHtml += `</div></div>`;

        this.innerHTML = headerHtml + listHtml;
    }
}


customElements.define('stm-header', Header);
customElements.define('stm-only-logo-header', OnlyLogoHeader);
customElements.define('stm-card', StmCard);
customElements.define('stm-footer', Footer);
customElements.define('stm-footer-continue', FooterWithContinueBtn);
customElements.define('stm-footer-previous', FooterPreviousScreenBtn);
customElements.define('stm-auth-task-header', AuthTaskHeader);
customElements.define('stm-auth-header', AuthHeader);
customElements.define('stm-auth-header-no-cus-name', AuthHeaderNoCusName);
customElements.define('stm-auth-header-auto-back-home', AuthHeaderNoCusName_BackToHome);
customElements.define('stm-cancel-trans-dialog', CancelTransOrNotDialog);
customElements.define('stm-service-not-avail-dialog', ServiceNotAvailableDialog);
customElements.define('stm-progress-bar', StmProgressBar);
customElements.define('stm-list', StmList);
customElements.define('stm-one-btn-dialog', OneButtonDialog);
customElements.define('stm-yes-no-dialog', YesOrNoDialog);
customElements.define('stm-table', CustomTable);