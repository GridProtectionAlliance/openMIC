/*! jquery-ui-scrolltab |  * v1.0.0 |  * https://davidsekar.github.io/jQuery-UI-ScrollTab/ |  * @license MIT */
(function ($) {
    $.widget('ui.scrollTabs', $.ui.tabs, {
        $ul: null,
        $leftArrowWrapper: null,
        $rightArrowWrapper: null,
        $scrollDiv: null,
        $navPrev: null,
        $navNext: null,
        $navFirst: null,
        $navLast: null,
        $innerWrapper: null,
        $headerWrapper: null,
        debounceEnabled: false,
        eventDelay: 200,
        sbarWidth: null,
        options: {
            scrollOptions: {
                closable: false,
                //customGotoFirstHTML: '<button class="stNavFirstArrow ui-state-active" title="First">' +
                //    '<span class="ui-icon ui-icon-seek-first">First tab</span></button>',
                //customMoveNextHTML: '<button class="stNavNextArrow ui-state-active" title="Next">' +
                //    '<span class="ui-icon ui-icon-carat-1-e">Next tab</span></button>',
                //customGoToLastHTML: '<button class="stNavLastArrow ui-state-active" title="Last">' +
                //    '<span class="ui-icon ui-icon-seek-end">Last tab</span></button>',
                //customMovePreviousHTML: '<button class="stNavPrevArrow ui-state-active" title="Previous">' +
                //    '<span class="ui-icon ui-icon-carat-1-w">Previous tab</span></button>',
                customGotoFirstHTML:
                    "<button class=\"btn btn-small btn-nav navbutton\" title=\"First\"><span class=\"glyphicon glyphicon-step-backward\"></span></button>",
                customMoveNextHTML:
                    "<button class=\"btn btn-small btn-nav navbutton\" title=\"Next\"><span class=\"glyphicon glyphicon-triangle-right\"></span></button>",
                customGoToLastHTML:
                    "<button class=\"btn btn-small btn-nav navbutton\" title=\"Last\"><span class=\"glyphicon glyphicon-step-forward\"></span></button>",
                customMovePreviousHTML:
                    "<button class=\"btn btn-small btn-nav navbutton\" title=\"Previous\"><span class=\"glyphicon glyphicon-triangle-left\"></span></button>",
                easing: 'swing',
                enableDebug: false,
                headerHTML: '<div class="ui-widget-header ui-corner-all"/>',
                headerScrollHTML: '<div class="ui-scroll-tabs-view"/>',
                hideDefaultArrows: false,
                leftArrowWrapperHTML: '<div class="stNavMain stNavMainLeft"/>',
                loadLastTab: false,
                onTabScroll: function () {
                    // empty
                },
                rightArrowWrapperHTML: '<div class="stNavMain stNavMainRight"/>',
                scrollSpeed: 500,
                selectTabAfterScroll: true,
                selectTabOnAdd: true,
                showFirstLastArrows: false,
                showNavWhenNeeded: true,
                wrapperCssClass: ''
            }
        },
        navigateOptions: {
            previous: 1,
            next: 2,
            first: 3,
            last: 4,
            mouseScroll: 5
        },
        _create: function () {
            var _this = this;
            var options = this.options;
            var $elem = this.element;
            this.$ul = $elem.find('ol,ul').eq(0).detach();
            /* Add custom markup */
            this.$headerWrapper = $(this.options.scrollOptions.headerHTML);
            this.$headerWrapper.addClass('ui-scroll-tabs-header');
            $elem.prepend(this.$headerWrapper);
            this.$innerWrapper = $(this.options.scrollOptions.headerScrollHTML);
            this.$innerWrapper.addClass('ui-scroll-tabs-view');
            this.$headerWrapper.append(this.$innerWrapper);
            this.$innerWrapper.append(this.$ul);
            /* End */
            /**
             * jQuery UI widget automatically adds the widget name to the triggered
             * events. So instead of 'tabsactivate' event, bind to 'scrolltabsactivate'.
             */
            this._on(this.element, {
                scrolltabsactivate: function (event, ui) {
                    _this._animateToActiveTab(event);
                }
            });
            // Call the base
            this._super();
            this._setupUserOptions();
            this._debug(this.eventNamespace);
        },
        /**
         * Method to identify the correct width of the scrollbar in different devices
         */
        _findScrollbarWidth: function () {
            var parent;
            var child;
            if (this.sbarWidth === null) {
                var style = document.createElement('style');
                style.innerHTML = '.__sb-test::-webkit-scrollbar { width: 0px; }';
                document.body.appendChild(style);
                parent = $('<div class="__sb-test" style="width:50px;height:50px;overflow:auto;">' +
                    '<div/></div>')
                    .appendTo('body');
                child = parent.children();
                this.sbarWidth = child.innerWidth() - child.height(99).innerWidth();
                // clean
                parent.remove();
                document.body.removeChild(style);
            }
        },
        _setOption: function (key, value) {
            this._super(key, value);
        },
        _setOptions: function (options) {
            this._super(options);
        },
        _activate: function (index) {
            this._super(index);
        },
        _setupUserOptions: function () {
            var options = this.options.scrollOptions;
            this.debounceEnabled = $.debounce ? true : false;
            this._debug('isDebounceEnabled : ' + this.debounceEnabled);
            var $elem = this.element;
            $elem.addClass(options.wrapperCssClass + ' ui-scroll-tabs');
        },
        /**
         * Centrally control all message to be logged to the console
         * @param message -message to be displayed
         */
        _debug: function (message, isError) {
            if (this.options.scrollOptions.enableDebug) {
                if (isError === true) {
                    console.error(message);
                }
                else {
                    console.log(message);
                }
            }
        },
        /**
         * If debounce/throttle plugin is found, it debounces the event handler function
         * @param dbFunc the event handler function
         */
        _debounceEvent: function (dbFunc) {
            return this.debounceEnabled ? $.debounce(this.eventDelay, dbFunc) : dbFunc;
        },
        /**
         * If debounce/throttle plugin is found, it uses it in the event handler function
         * @param dbFunc the event handler function
         */
        _throttleEvent: function (dbFunc) {
            return this.debounceEnabled ? $.throttle(this.eventDelay, dbFunc) : dbFunc;
        },
        _bindMouseScroll: function () {
            var _this = this;
            if ($.isFunction($.fn.mousewheel)) {
                var self_1 = this;
                this._on(this.$scrollDiv, {
                    mousewheel: function (event) {
                        event.preventDefault();
                        self_1._scrollWithoutSelection(_this.navigateOptions.mouseScroll, event.deltaY * event.deltaFactor * 3.5);
                        // self._debug(event.deltaX + ',' + event.deltaY + ',' + event.deltaFactor);
                    }
                });
            }
        },
        /**
         * Initializes the navigation controls based on user settings
         */
        _setupNavControls: function () {
            var _this = this;
            this.$scrollDiv = this.$ul.parent();
            // Set the height of the UL
            // this.$scrollDiv.height(this.tabs.first().outerHeight());
            this.$leftArrowWrapper = $(this.options.scrollOptions.leftArrowWrapperHTML);
            this.$rightArrowWrapper = $(this.options.scrollOptions.rightArrowWrapperHTML);
            if (!this.options.scrollOptions.hideDefaultArrows) {
                this.$navPrev = $(this.options.scrollOptions.customMovePreviousHTML).button();
                this.$leftArrowWrapper.append(this.$navPrev);
                this.$navNext = $(this.options.scrollOptions.customMoveNextHTML).button();
                this.$rightArrowWrapper.append(this.$navNext);
                if (this.options.scrollOptions.showFirstLastArrows === true) {
                    this.$navFirst = $(this.options.scrollOptions.customGotoFirstHTML).button();
                    this.$leftArrowWrapper.prepend(this.$navFirst);
                    this.$navLast = $(this.options.scrollOptions.customGoToLastHTML).button();
                    this.$rightArrowWrapper.append(this.$navLast);
                }
                else {
                    this.$navFirst = this.$navLast = $();
                }
            }
            this.$scrollDiv.before(this.$leftArrowWrapper);
            this.$scrollDiv.after(this.$rightArrowWrapper);
            this._debug('showNavWhenNeeded ' + this.showNavWhenNeeded);
            if (this.options.scrollOptions.showNavWhenNeeded) {
                this.$headerWrapper.addClass('show-controls-when-required');
            }
            else {
                this.$headerWrapper.addClass('show-controls-always');
            }
            this._addclosebutton();
            this._bindMouseScroll();
            // Triggers on the scroll end
            this._on(this.$scrollDiv, {
                scroll: this._debounceEvent(function () {
                    _this._showNavsIfNeeded();
                })
            });
        },
        /**
         * Initializes all the controls and events required for scroll tabs
         */
        _init: function () {
            var _this = this;
            this._setupNavControls();
            setTimeout(function () {
                _this._showNavsIfNeeded();
            }, this.eventDelay);
            this._hideScrollBars();
            this._addNavEvents();
            this._on(window, {
                resize: this._debounceEvent(function () {
                    _this._debug('resize: ' + _this.eventNamespace);
                    _this._showNavsIfNeeded();
                })
            });
        },
        _hideScrollBars: function () {
            this._findScrollbarWidth();
            this.$innerWrapper.css('margin-bottom', -1 * this.sbarWidth);
        },
        /**
         * Check if navigation need then show; otherwise hide it
         */
        _showNavsIfNeeded: function () {
            var showLeft = !(this.$scrollDiv.scrollLeft() <= 0);
            var showRight = !(Math.abs(this.$scrollDiv[0].scrollWidth - this.$scrollDiv.scrollLeft() - this.$scrollDiv.outerWidth()) < 1);

            if (this.options.scrollOptions.selectTabAfterScroll) {
                // JRC: Corrected behavior 8/10/21
                showLeft = (this.options.active === 0) ? false : showLeft;
                showRight = (this.options.active + 1 === this.tabs.length) ? false : showRight;
            }

            showLeft ? this.$leftArrowWrapper.addClass('stNavVisible')
                : this.$leftArrowWrapper.removeClass('stNavVisible');

            showRight ? this.$rightArrowWrapper.addClass('stNavVisible')
                : this.$rightArrowWrapper.removeClass('stNavVisible');

            if (this.options.scrollOptions.showNavWhenNeeded === false) {
                this.$navFirst.button('option', 'disabled', !showLeft);
                this.$navPrev.button('option', 'disabled', !showLeft);
                this.$navLast.button('option', 'disabled', !showRight);
                this.$navNext.button('option', 'disabled', !showRight);
                if (showLeft || showRight) {
                    this.$headerWrapper.addClass('st-activate-controls');
                }
                else {
                    this.$headerWrapper.removeClass('st-activate-controls');
                }
            }
            this._debug('Validate showing nav controls');
        },
        _callBackFnc: function (fName, event, arg1) {
            if ($.isFunction(fName)) {
                fName(event, arg1);
            }
        },
        /**
         * returns the delta that should be added to current scroll to bring it into view
         * @param $tab tab that should be tested
         */
        _getScrollDeltaValue: function ($tab) {
            var leftPosition = $tab.position();
            var width = $tab.outerWidth();
            var currentScroll = this.$scrollDiv.scrollLeft();
            var currentVisibleWidth = this.$scrollDiv.width();
            var hiddenDirection = 0;
            var arrowsWidth = this.$leftArrowWrapper.width();
            // Check if the new tab is in view
            if (leftPosition.left < (currentScroll + arrowsWidth)) {
                hiddenDirection = leftPosition.left - currentScroll - arrowsWidth;
            }
            else if ((leftPosition.left + width + arrowsWidth) > (currentScroll + currentVisibleWidth)) {
                hiddenDirection = (leftPosition.left + width + arrowsWidth)
                    - (currentScroll + currentVisibleWidth);
            }
            return hiddenDirection;
        },
        _scrollWithoutSelection: function (navOpt, sLeft) {
            var scrollLeft = this.$scrollDiv.scrollLeft();
            switch (navOpt) {
                case this.navigateOptions.first:
                    scrollLeft = 0;
                    break;
                case this.navigateOptions.last:
                    scrollLeft = this.$scrollDiv[0].scrollWidth;
                    break;
                case this.navigateOptions.previous:
                    scrollLeft -= this.$scrollDiv.outerWidth() / 2;
                    break;
                case this.navigateOptions.next:
                    scrollLeft += this.$scrollDiv.outerWidth() / 2;
                    break;
                case this.navigateOptions.mouseScroll:
                    scrollLeft += sLeft;
                    break;
            }
            if (scrollLeft < 0) {
                scrollLeft = 0;
            }
            this.$scrollDiv.stop().animate({ scrollLeft: scrollLeft }, this.options.scrollOptions.scrollSpeed / 2, this.options.scrollOptions.easing);
        },
        _animateToActiveTab: function (e) {
            var calculatedDelta = this._getScrollDeltaValue(this.active);
            this.$scrollDiv.stop().animate({ scrollLeft: this.$scrollDiv.scrollLeft() + calculatedDelta }, this.options.scrollOptions.scrollSpeed, this.options.scrollOptions.easing);
            this._showNavsIfNeeded();
            // trigger callback if defined
            e = (typeof e === 'undefined') ? null : e;
            this._callBackFnc(this.options.scrollOptions.onTabScroll, e, this.active);
        },
        /**
         * Return a new jQuery object for user provided selectors or else use the default ones
         * @param col if selector is provided by user, then override the existing controls
         * @param nav Nav control selector option prop name suffix
         */
        _getCustomNavSelector: function (col, nav) {
            var sel = this.options.scrollOptions['customNav' + nav] || '';
            // Check for custom selector
            if (typeof sel === 'string' && $.trim(sel) !== '') {
                col = col.add(sel);
            }
            return col;
        },
        /**
         * This function add the navigation control and binds the required events
         */
        _addNavEvents: function () {
            var _this = this;
            // Handle next tab
            this.$navNext = this.$navNext || $();
            this.$navNext = this._getCustomNavSelector(this.$navNext, 'Next');
            this._on(this.$navNext, {
                click: this._debounceEvent(function (e) { _this._moveToNextTab(e); })
            });
            // Handle previous tab
            this.$navPrev = this.$navPrev || $();
            this.$navPrev = this._getCustomNavSelector(this.$navPrev, 'Prev');
            this._on(this.$navPrev, {
                click: this._debounceEvent(function (e) { _this._moveToPrevTab(e); })
            });
            // Handle First tab
            this.$navFirst = this.$navFirst || $();
            this.$navFirst = this._getCustomNavSelector(this.$navFirst, 'First');
            this._on(this.$navFirst, {
                click: this._debounceEvent(function (e) { _this._moveToFirstTab(e); })
            });
            // Handle last tab
            this.$navLast = this.$navLast || $();
            this.$navLast = this._getCustomNavSelector(this.$navLast, 'Last');
            this._on(this.$navLast, {
                click: this._debounceEvent(function (e) { _this._moveToLastTab(e); })
            });
        },
        /**
         * Handles move to next tab link click
         * @param e pass the link click event
         */
        _moveToNextTab: function (e) {
            if (e) {
                e.preventDefault();
            }
            if (!this.options.scrollOptions.selectTabAfterScroll) {
                this._scrollWithoutSelection(this.navigateOptions.next);
                return;
            }
            this._activate(this._findNextTab(Math.max(0, this.options.active + 1), true));
        },
        /**
         * Handles move to previous tab link click
         * @param e pass the link click event
         */
        _moveToPrevTab: function (e) {
            if (e) {
                e.preventDefault();
            }
            if (!this.options.scrollOptions.selectTabAfterScroll) {
                this._scrollWithoutSelection(this.navigateOptions.previous);
                return;
            }
            this._activate(this._findNextTab(Math.max(0, this.options.active - 1), false));
        },
        /**
         * Handles move to first tab link click
         * @param e pass the link click event
         */
        _moveToFirstTab: function (e) {
            if (e) {
                e.preventDefault();
            }
            if (!this.options.scrollOptions.selectTabAfterScroll) {
                this._scrollWithoutSelection(this.navigateOptions.first);
                return;
            }
            this._activate(this._findNextTab(0, false));
        },
        /**
         * Handles move to last tab link click
         * @param e pass the link click event
         */
        _moveToLastTab: function (e) {
            if (e) {
                e.preventDefault();
            }
            if (!this.options.scrollOptions.selectTabAfterScroll) {
                this._scrollWithoutSelection(this.navigateOptions.last);
                return;
            }
            this._activate(this._findNextTab(this.tabs.length - 1, true));
        },
        _addclosebutton: function ($li) {
            var _this = this;
            if (this.options.scrollOptions.closable === false) {
                return;
            }
            // If li is provide than just add to that, otherwise add to all
            var lis = $li || this.tabs;
            var self = this;
            lis.each(function (idx, obj) {
                var $thisLi = $(obj).addClass('stHasCloseBtn');
                $thisLi.append($('<span class="ui-state-default ui-corner-all stCloseBtn">' +
                    '<span class="ui-icon ui-icon-circle-close" title="Close this tab">Close</span>' +
                    '</span>'));
                var closeButton = $thisLi.find('.stCloseBtn').hover(function () {
                    $(this).toggleClass('ui-state-hover');
                });
                _this._on(closeButton, {
                    click: function (e) {
                        var removeIdx = self.tabs.index($thisLi);
                        var selectTabIdx;
                        selectTabIdx = -1;
                        if (self.options.active === removeIdx) {
                            var tabcount = self.tabs.length;
                            var mid = Math.ceil(tabcount / 2);
                            if (removeIdx > mid) {
                                selectTabIdx = removeIdx - 1;
                            }
                            else {
                                selectTabIdx = removeIdx;
                            }
                        }
                        self.removeTab($thisLi.find('a.ui-tabs-anchor'));
                        if (selectTabIdx > -1 && selectTabIdx !== removeIdx) {
                            self._activate(selectTabIdx);
                        }
                    }
                });
            });
        },
        addTab: function (header, panelContent) {
            var newId = $({}).uniqueId()[0].id;
            var tab = $('<li><a href="#' + newId + '" role="tab">' + header + '</a></li>');
            var panel = this._createPanel(newId);
            panel.html(panelContent);
            this.$ul.append(tab);
            panel.attr('aria-live', 'polite');
            if (panel.length) {
                $(this.panels[this.panels.length - 1]).after(panel);
            }
            panel.attr('role', 'tabpanel');
            this._addclosebutton(tab);
            this.refresh();
            if (this.options.scrollOptions.selectTabOnAdd) {
                this._moveToLastTab();
            }
            this._showNavsIfNeeded();
        },
        removeTab: function (anc) {
            var tabId = anc.attr('href');
            // Remove the panel
            $(tabId).remove();
            // Remove the tab
            anc.closest('li').remove();
            // Refresh the tabs widget
            this.refresh();
        },
        _destroy: function () {
            this._super();
            /* Remove navigation controls */
            this.$leftArrowWrapper.remove();
            this.$rightArrowWrapper.remove();
            /* undo the close button */
            this.tabs.each(function (idx, element) {
                var self = $(element);
                self.removeClass('stHasCloseBtn');
                self.find('.stCloseBtn').remove();
            });
            /* Undo enhanced tab headers */
            var $headerWrapper = this.$ul.closest('.ui-scroll-tabs-header');
            this.$ul = this.$ul.detach();
            $headerWrapper.remove();
            this.element.prepend(this.$ul);
            /* Remove class from the base wrapper */
            this.element.removeClass(this.options.wrapperCssClass)
                .removeClass('ui-scroll-tabs');
            /* unsubscribe all events in namespace */
            // this._off(window, 'resize');
            $(window).off('resize' + this.eventNamespace);
        }
    });
    return $.ui.scrollTabs;
})(jQuery);