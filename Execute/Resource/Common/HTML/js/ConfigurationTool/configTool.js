new Vue({
    el: '#app',
    data: {
        isModalVisible: false,
        alertMessage: '',
        showAlertBox: false,
        isConfirmVisible: false,
        confirmMessage: '',

        backPage: 'Main.html',
        profileImg:"../../images/ConfigurationTool/DeviceImg.png",

        currentSelectType: '',
        selectedATMType: '',
        atmTypes: [],

        selectedId: null,
        selectedChildId: null,

        menus:[
            {id: 'common', label: 'Common', isDropdownOpen: true},  
            {id: 'xdc', label: 'XDC', isDropdownOpen: true},
            {id: 'iso8583', label: 'ISO8583', isDropdownOpen: true}
        ], 
        settings: {
            common: [
                { id: 'MainSetting', icon: '⚙️', parent: 'common', label: 'Main Setting', page: 'MainSetting.html', isDropdownOpen: false },
                { id: 'DeviceSetting', icon: '⚙️', parent: 'common', label: 'Device Setting', page: 'DeviceSetting.html' },
                { id: 'ServicesSetting', icon: '⚙️', parent: 'common', label: 'Services Setting', page: 'ServicesSetting.html' },
                { id: 'NetworkSetting', icon: '⚙️', parent: 'common', label: 'Network Setting', page: 'NetworkSetting.html', isDropdownOpen: false },
                { id: 'DataFormatterSetting', icon: '⚙️', parent: 'common', label: 'Data Formatter Setting', page: 'DataFormatterSetting.html', isDropdownOpen: false },
            ],
            xdc: [
                { id: 'MessageSetting', icon: '⚙️', parent: 'xdc', label: 'Message Setting', page: 'MessageSetting.html' },
                { id: 'DevicePropertySetting', icon: '⚙️', parent: 'xdc', label: 'Device Property Setting', page: 'DevicePropertySetting.html', isDropdownOpen: false },
                { id: 'HostDenoSetting', icon: '⚙️', parent: 'xdc', label: 'Host Denomination Setting', page: 'HostDenoSetting.html' },
                { id: 'CassetteSetting', icon: '⚙️', parent: 'xdc', label: 'Cassette Setting', page: 'CassetteSetting.html' },
                { id: 'NoCardSetting', icon: '⚙️', parent: 'xdc', label: 'No Card Setting', page: 'NoCardSetting.html' },
                { id: 'ScreenSetting', icon: '⚙️', parent: 'xdc', label: 'Screen Setting', page: 'ScreenSetting.html', isDropdownOpen: false },
            ],
            iso8583: [
                { id: 'MainFor8583Setting', icon: '⚙️', parent: 'xdc', label: 'Main Setting', page: 'MainFor8583Setting.html' },
            ],
        },

        content: '',
        setting: '',
        originSettingData: {},
        settingData: {
            System: [],
            Language: [],
            Door: [],
            Others: [],
            Configuration: [],
            FontCfg: [],
            Sessions: [],
            DataFormatters: [],
            CassetteConfig: [],
            DenoConfig: [],
        },
        itemChild: '',
        itemChilds: {
            content: [],
            owner: '',
        },
        currency: ['AED', 'ARS', 'AUD', 'BRL', 'BYR', 'CAD', 'CHF', 'CLP', 'CNY', 'COP', 'CUC', 'CZK', 'DKK', 'EGP', 'ESP', 'EUR', 'GBP', 'GHC', 'HKD', 'HRK', 'LEI', 'IDR', 'INR', 'IQD', 'IRR', 'JOD', 'JPY', 'KRW', 'KWD', 'KZT', 'LAK', 'LBP', 'LKR', 'MAD', 'MXN', 'MYR', 'NOK', 'NZD', 'PLN', 'QAR', 'RON', 'RUB', 'SAR', 'SEK', 'SDG', 'SGD', 'SIT', 'SKK', 'THB', 'TRL', 'TWD', 'USD', 'UYU', 'VEB', 'VND', 'XAF', 'XXG', 'XXN', 'ZAR', 'PHP'],
        priorityDevices: ['PIN', 'SIU', 'CDM', 'CIM', 'RPTR', 'IDC', 'CRDIDC'],
        sortedDevices: [],
    },
    methods: {
        sortDevicesByPriority(devices, priorityDevices) {
            return devices.sort((a, b) => {
                const keyA = a.DeviceAlias || a.DeviceName;
                const keyB = b.DeviceAlias || b.DeviceName;

                const indexA = priorityDevices.indexOf(keyA);
                const indexB = priorityDevices.indexOf(keyB);

                // 如果 keyA 或 keyB 不在 priorityDevices 中，则排在后面
                if (indexA === -1) return 1;
                if (indexB === -1) return -1;

                // 按照 priorityDevices 中的顺序排序
                return indexA - indexB;
            });
        },
        getPage(page) {
            console.log('GET:' + 'http://localhost:5674/showPage/' + page);
            fetch('http://localhost:5674/showPage/' + page)
                .then(response => response.json())
                .then(data => {
                    console.log('GET:' + 'http://localhost:5674/showPage/' + page + 'success');
                    console.log(data);
                })
                .catch(error => {
                    console.error('catch error:', error);
                });
        },
        readData() {
            let data = {
                "request": "read" + this.setting
            };
            console.log('POST:' + 'http://localhost:5674/, request: ' + "read" + this.setting);
            fetch('http://localhost:5674/', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('network response error');
                    }
                    return response.json();
                })
                .then(data => {
                    console.log('POST:' + 'http://localhost:5674/, request: ' + "read" + this.setting + 'success');              
                    this.originSettingData = JSON.parse(JSON.stringify(data.data));//保存原始数据
                    this.settingData = data.data;
                    console.log(this.settingData);

                    // 如果是 MainSetting，将 Door 对象中的字符串 "0"/"1" 转换为布尔值
                    if (this.setting === 'MainSetting' && this.settingData?.Door) {
                        this.convertDoorStringToBoolean(this.settingData.Door);
                    }

                    if (this.setting === 'BankSetting') {
                        this.atmTypes = data.data;
                        if (this.atmTypes.length > 0) {
                            this.selectedATMType = this.atmTypes[0];
                        }
                    } else {
                        // 优先使用响应中的 ProductName，如果为空则从 sessionStorage 恢复或保持当前值
                        const productName = data.msg.ProductName?.trim();
                        if (productName) {
                            this.selectedATMType = productName;
                            sessionStorage.setItem('selectedATMType', productName);
                        } else {
                            // 如果响应中没有 ProductName，尝试从 sessionStorage 恢复
                            const savedType = sessionStorage.getItem('selectedATMType');
                            if (savedType) {
                                this.selectedATMType = savedType;
                            }
                            // 如果都没有，保持当前值（可能是用户刚选择的类型）
                        }
                        this.atmTypes = data.msg.ProductList;
                        this.itemChilds.content = data.msg.SettingItems;
                        this.itemChilds.owner = this.setting;

                        if (this.itemChilds.content?.length > 0) {
                            this.setDropdownOpenById(this.setting);
                        }

                        if (this.setting === 'CassetteSetting') {
                            if (this.settingData?.CassetteConfig?.length > 0) {
                                this.settingData.CassetteConfig.forEach(cassette => { this.updateCassetteValues(cassette); });
                            }
                        } else if (this.setting === 'DeviceSetting') {
                            this.sortedDevices = this.sortDevicesByPriority(this.settingData, this.priorityDevices);
                        }
                    }


                })
                .catch(error => {
                    console.error('catch error:', error);
                });
        },
        writeData() {
            //如果是MainSetting，将Door bool->string
            let dataToWrite = JSON.parse(JSON.stringify(this.settingData));
            if (this.setting === 'MainSetting' && dataToWrite?.Door) {
                this.convertDoorBooleanToString(dataToWrite.Door);
            }

            let data = {
                "request": "write" + this.setting,
                "data": dataToWrite
            };
            console.log('POST:' + 'http://localhost:5674/, request: ' + "write" + this.setting);
            fetch('http://localhost:5674/', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('network response error');
                    }
                    return response.json();
                })
                .then(data => {
                    console.log('POST:' + 'http://localhost:5674/, request: ' + "write" + this.setting + 'success');
                    console.log(data);
                    if (data.data == "modify success") {
                        this.readData();
                        this.showAlert('Modify success');
                    }
                })
                .catch(error => {
                    console.error('catch error:', error);
                });
        },
        loadATMConfig(isApplyDefault) {
            console.log('GET:' + `http://localhost:5674/loadConfig?type=${this.selectedATMType}&applyDefault=${isApplyDefault}`);
            return fetch(`http://localhost:5674/loadConfig?type=${this.selectedATMType}&applyDefault=${isApplyDefault}`)
                .then(response => response.json())
                .then(data => {
                    console.log('GET:' + 'http://localhost:5674/loadConfig/' + this.selectedATMType + 'success');
                    console.log(data);
                    this.readData();//刷新页面数据
                })
                .catch(error => {
                    console.error('catch error:', error);
                });
        },
        convertDoorStringToBoolean(door) {
            if (door.enableOperatorSwitch !== undefined) {
                door.enableOperatorSwitch = door.enableOperatorSwitch === "1" || door.enableOperatorSwitch === 1 || door.enableOperatorSwitch === true;
            }
            if (door.enableCabinet !== undefined) {
                door.enableCabinet = door.enableCabinet === "1" || door.enableCabinet === 1 || door.enableCabinet === true;
            }
            if (door.enableSafeDoor !== undefined) {
                door.enableSafeDoor = door.enableSafeDoor === "1" || door.enableSafeDoor === 1 || door.enableSafeDoor === true;
            }
        },
        convertDoorBooleanToString(door) {
            if (door.enableOperatorSwitch !== undefined) {
                door.enableOperatorSwitch = door.enableOperatorSwitch ? "1" : "0";
            }
            if (door.enableCabinet !== undefined) {
                door.enableCabinet = door.enableCabinet ? "1" : "0";
            }
            if (door.enableSafeDoor !== undefined) {
                door.enableSafeDoor = door.enableSafeDoor ? "1" : "0";
            }
        },
        updateCassetteValues(cassette) {
            if (this.settingData.DenoConfig != null && this.settingData.DenoConfig.length > 0) {
                const selectedOption = this.settingData.DenoConfig.find(type => type.name === cassette.hostCassetteType);
                if (selectedOption) {
                    cassette.currency = selectedOption.currency;
                    cassette.denomination = selectedOption.denomination;
                }
            }
        },
        setDropdownOpenById(id) {
            for (let category in this.settings) {
                const foundItem = this.settings[category].find(item => item.id === id);
                if (foundItem) {
                    foundItem.isDropdownOpen = true;
                    return;
                }
            }
        },
        validateDevicePropertyInput(device, key, event) {
            const newValue = event.target.value; //当前输入的值
            // 获取原始值
            const originDevice = this.originSettingData.Devices?.find(item => item.DeviceName === device.DeviceName);
            if (!originDevice) {
                console.error(`Device ${device.DeviceName} not found in originSettingData`);
                return;
            }       
            const originalValue = originDevice.DeviceProperty[key];
      
            // 校验输入值是否为 0 或 1
            if(device.DeviceName == "CashDeposit" || device.DeviceName == "CashDispenser" 
                || device.DeviceName == "PrinterCfg" || device.DeviceName == "IPM"){
                    if (newValue !== '0' && newValue !== '1') {
                        this.showAlert('Only 0 or 1 is allowed');
                        event.target.value = originalValue;
                        device.DeviceProperty[key] = originalValue; 
                      }
                }
        },
        showModal() {
            this.isModalVisible = true;
        },
        hideModal() {
            this.isModalVisible = false;
        },
        mainConfirm(isApplyDefault) {
            if (this.selectedATMType != null && this.selectedATMType != '') {
                // 先保存 selectedATMType 到 sessionStorage，防止页面加载时丢失
                sessionStorage.setItem('selectedATMType', this.selectedATMType);
                this.loadATMConfig(isApplyDefault).then(() => {
                    // 等待配置加载完成后再导航到主页面
                    this.getPage("MainSetting.html");
                    sessionStorage.setItem('selectedId', 'MainSetting');
                });
            } else {
                this.getPage("MainSetting.html");
                sessionStorage.setItem('selectedId', 'MainSetting');
            }
            this.hideModal();
        },
        confirmSelection(isApplyDefault) {
            this.hideModal();
            if (this.currentSelectType != null && this.currentSelectType != '') {
                this.selectedATMType = this.currentSelectType;
                // 保存 selectedATMType 到 sessionStorage
                sessionStorage.setItem('selectedATMType', this.selectedATMType);
                this.loadATMConfig(isApplyDefault);
            }
        },

        showConfirmation(message) {
            this.confirmMessage = message;
            this.isConfirmVisible = true;
        },
        cancelConfirmation() {
            this.isConfirmVisible = false;
        },
        confirmAction() {
            this.isConfirmVisible = false;
            this.writeData();
        },

        confirm() {
            this.showConfirmation("Are you sure to apply the settings?");
        },
        atmTypesConfirm() {
            this.showConfirmation("Are you sure to apply the default settings?");
        },
        showAlert(message) {
            this.alertMessage = message;
            this.showAlertBox = true;
        },
        closeAlert() {
            this.showAlertBox = false;
        },
        toggleDropdown(category) {
            const menuItem = this.menus.find(menu => menu.id === category);
            if (menuItem) {
                menuItem.isDropdownOpen = !menuItem.isDropdownOpen;
            }
        },
        toggleSelection(item) {
            item.isDropdownOpen = !item.isDropdownOpen;
            if (this.selectedId !== item.id) {
                item.isDropdownOpen = !item.isDropdownOpen;
                this.selectedId = item.id;
                sessionStorage.setItem('selectedId', item.id);
                //初始化itemChilds
                this.itemChilds = {
                    content: [],
                    owner: ''
                };
                this.getPage(item.page);
            }
        },
        toggleChildSelection(childId) {
            if (childId) {
                if (this.selectedChildId !== childId) {
                    this.selectedChildId = childId;
                }
                //滚动到对应位置
                const targetId = `content-${childId}`;
                const targetElement = document.getElementById(targetId);

                if (targetElement) {
                    const elementPosition = targetElement.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.scrollY; //获取相对于文档的位置    
                    const offset = 80;//计算偏移量          
                    //滚动到目标位置
                    window.scrollTo({
                        top: offsetPosition - offset,
                        behavior: 'smooth'
                    });
                } else {
                    console.error(`Element with id ${targetId} not found.`);
                }

            }
        },
        handleVisibilityChange() {
            if (document.hidden) {
                sessionStorage.setItem('sidebarScrollY', this.$refs.sidebar.scrollTop);
            }
        },
        toggleEnabled(data, selectItem, key2Find, key2Change) {
            const selectedOption = data.find(type => type[key2Find] === selectItem);
            if (selectedOption) {
                selectedOption[key2Change] = selectedOption[key2Change] === "0" ? "1" : "0";
            }
        },
        getStyle(id) {
            return {
                backgroundColor: this.selectedId === id ? '#cce5ff' : '',
                color: this.selectedId === id ? '#004dff' : '',
                padding: '10px',
                borderRadius: '5px'
            };
        },
        initHeader() {
            const header = document.querySelector('.header');
            if (!header) {return;}

            let headerHtml = '<div class="logo-section"> \n' +
                '<img src="../../images/ConfigurationTool/Logo.png" alt="Logo"> \n' +
                '<span class="title">eCAT Configuration Tool</span> \n' +
                '</div> \n' +
                '<div class="header-buttons"> \n' +
                '<button id="btn-header1">-</button> \n' +
                '<button id="btn-header2">+</button> \n' +
                '<button id="btn-header3">x</button> \n' +
                '</div> \n' +
                '</div> \n';
            header.innerHTML = headerHtml;
            const minimizeBtn = document.querySelector('#btn-header1');
            const maximizeBtn = document.querySelector('#btn-header2');
            const closeBtn = document.querySelector('#btn-header3');

            minimizeBtn?.addEventListener('click', function(){
                window.chrome.webview.postMessage({ msgType: 'event', name: 'minimizeClick' });
            });
            maximizeBtn?.addEventListener('click', function(){
                window.chrome.webview.postMessage({ msgType: 'event', name: 'maximizeClick' });
            });
            closeBtn?.addEventListener('click', function(){
                window.chrome.webview.postMessage({ msgType: 'event', name: 'closeClick' });
            });         
        },

        initBottomButtons() {
            const bottomButtons = document.querySelector('.buttons-bottom');
            if (!bottomButtons) {return;}
            let bottomButtonsHtml = '<button class="btn-reset">Reset</button> \n' +
                '<button class="btn-confirm">Confirm</button> \n';
            bottomButtons.innerHTML = bottomButtonsHtml;
            const btn1 = document.querySelector('.btn-reset');
            const btn2 = document.querySelector('.btn-confirm');

            btn1?.addEventListener('click', this.readData.bind(this));
            btn2?.addEventListener('click', this.confirm.bind(this));
        }
    },
    mounted() {
        this.initHeader();
        this.initBottomButtons();
        document.addEventListener('visibilitychange', this.handleVisibilityChange);
        const scrollY = sessionStorage.getItem('sidebarScrollY');
        if (scrollY) {
            this.$nextTick(() => {
                this.$refs.sidebar.scrollTop = parseInt(scrollY, 10);
            });
        }
        //读取当前选择项
        const selectedId = sessionStorage.getItem('selectedId');
        if (selectedId) {
            this.selectedId = selectedId;
        }

        //读取保存的 ATM 类型
        const savedATMType = sessionStorage.getItem('selectedATMType');
        if (savedATMType) {
            this.selectedATMType = savedATMType;
        }

        //读取当前页面数据
        let currentSetting = document.getElementById('app').getAttribute('currentSetting');
        if (currentSetting !== '') {
            this.setting = currentSetting;
            this.readData();
        }
    },
    beforeDestroy() {
        document.removeEventListener('visibilitychange', this.handleVisibilityChange);
    },
});