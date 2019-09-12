(function ($) {
    // Method expose for user use
    var methods = {
        init: function (opts) {
            opts = $.extend({}, $.fn.multiSelectTreeView.defaults, opts);
            return this.each(function () {
                return new multiSelectTreeView(this, opts);
            });
        },
        getSelectedItems: function () {
            var $me = $(this);
            var items = $me.find('input[data-is-child=true]:checked'),
                totalItems = items.length,
                values = [];
            for (var i = 0; i < totalItems; i++) {
                values.push(items[i].value);
            }

            return values.join(',');
        }
    };

    // Plugin
    $.fn.multiSelectTreeView = function (options) {
        if (methods[options]) {
            return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return methods.init.apply(this, arguments);
        }
        else {
            throw 'Method does not supported!';
        }
    };

    // Default options
    $.fn.multiSelectTreeView.defaults = {
        noneSelectedText: 'Select options',
        selectedText: '# selected',
        selectedAllText: 'All selected',
        disabled: false,
        isSelectedAll: false,
        data: [],
        selectedItems: '',
        onSelect: function () { }
    };

    var multiSelectTreeView = function (multiSelect, options) {
        var me = this;
        me.options = options;
        me.id = multiSelect.id;
        me.$multiSelect = $(multiSelect);

        me.initialize();

        return me.$multiSelect;
    };

    multiSelectTreeView.prototype = {
        initialize: function () {
            var me = this,
                id = me.id;
            me.isOpenning = false;
            me.bodyId = 'multiselect-treeview-' + id;

            var html = me.render();

            //add global variable
            me.$multiSelect = me.$multiSelect.html(html);
            me.$dropdownbtn = me.$multiSelect.find('.multiselect-treeview-dropdown-button').first();
            me.$updownicon = me.$multiSelect.find('.up-down-icon').first();
            me.$selectedtext = me.$multiSelect.find('.selected-text').first();
            me.$dropdownlist = me.$multiSelect.find('.multiselect-treeview-dropdown-list').first();
            me.$checkboxwrappers = me.$multiSelect.find('.checkbox-wrapper');
            me.$selectAll = me.$multiSelect.find('#selectAll');
            me.$checkboxes = me.$multiSelect.find('.multiselect-treeview-items input[type=checkbox]');

            me.registerEvents();
            me.updateSelectedItems();
        },
        render: function () {
            var me = this,
                options = me.options;
            return '<div class="multiselect-treeview">'
                + '<div class="multiselect-treeview-dropdown-button ' + (options.disabled ? 'disabled' : '') + '">'
                + '<span class="selected-text">' + options.noneSelectedText + '</span>'
                + '<span class="up-down-icon"><span class="down"></span></span>'
                + '</div>'
                + '<div class="multiselect-treeview-dropdown-list hidden">'
                + '<div class="multiselect-treeview-select-all">'
                + '<div class="checkbox-wrapper">'
                + '<input type="checkbox" id="selectAll" ' + (options.isSelectedAll ? 'checked' : '') + ' />'
                + '<label for= "selectAll" > Select all</label>'
                + '</div>'
                + '</div>'
                + me.renderItems()
                + '</div>' +
                '</div>';
        },
        renderItems: function () {
            var me = this,
                options = me.options,
                html = [],
                totalItems = options.data.length;
            html.push('<div class="multiselect-treeview-items">');
            for (var i = 0; i < totalItems; i++) {
                var data = options.data[i];
                data.hasChild = data.items && data.items.length > 0;
                var itemId = 'item-' + i;

                html.push('<ul>'
                    + '<li class="multiselect-treeview-item">'
                    + '<div class="checkbox-wrapper">'
                    + '<input type="checkbox" id="' + itemId + '" value="' + data.id + '" data-is-child="' + data.isChild + '" '
                    + (data.checked || options.isSelectedAll ? 'checked' : '') + ' />'
                    + '<label for="' + itemId + '">' + data.text + '</label>'
                    + '</div>');

                if (data.hasChild) {
                    html.push(me.renderChilds({
                        data: data.items,
                        parentId: itemId
                    }));
                }

                html.push('</li></ul>');
            }

            html.push('</div>');
            return html.join('');
        },
        renderChilds: function (item) {
            var me = this,
                options = me.options,
                html = [],
                totalItems = item.data.length;
            html.push('<ul>');
            for (var i = 0; i < totalItems; i++) {
                var data = item.data[i];
                data.hasChild = data.items && data.items.length > 0;
                var itemId = item.parentId + '-' + i;
                html.push('<li class="multiselect-treeview-item is-child">'
                    + '<div class="checkbox-wrapper">'
                    + '<input type="checkbox" id="' + itemId + '" value="' + data.id + '"' + '" data-is-child="' + data.isChild + '" '
                    + ' data-parentid="' + item.parentId + '"'
                    + ' ' + (data.checked || options.isSelectedAll ? 'checked' : '') + ' />'
                    + '<label for="' + itemId + '">' + data.text + '</label>'
                    + '</div>');

                if (data.hasChild) {
                    html.push(me.renderChilds({
                        data: data.items,
                        parentId: itemId
                    }));
                }

                html.push('</li>');
            }

            html.push('</ul>');
            return html.join('');
        },
        updateSelectedText: function () {
            var me = this,
                options = me.options,
                $checkboxes = me.$checkboxes.filter('[data-is-child=true]'),
                $checked = $checkboxes.filter(':checked');

            if ($checked.length === 0) {
                me.$selectedtext.text(options.noneSelectedText);
                me.$selectAll.prop('checked', false);
            } else {
                if ($checkboxes.length === $checked.length) {
                    me.$selectedtext.text(options.selectedAllText);
                    me.$selectAll.prop('checked', true);
                } else {
                    me.$selectedtext.text(options.selectedText.replace('#', $checked.length));
                    me.$selectAll.prop('checked', false);
                }
            }
        },
        updateSelectedItems: function () {
            var me = this,
                options = me.options,
                checkboxes = me.$checkboxes.filter('[data-is-child=true]'),
                totalItems = checkboxes.length;

            if (options.selectedItems && options.selectedItems.length > 0) {
                var selectedItems = options.selectedItems.split(',');
                for (var i = 0; i < totalItems; i++) {
                    var $checkbox = $(checkboxes[i]);
                    var index = selectedItems.findIndex(x => x === $checkbox.val());
                    if (index >= 0) {
                        $checkbox.prop('checked', true).change();
                    }
                }
            } else {
                me.updateSelectedText();
            }
        },
        registerEvents: function () {
            var me = this,
                options = me.options;

            var updateParent = function () {
                var $this = $(this),
                    parentId = $this.data('parentid');

                if (parentId) {
                    var checked = $this.prop('checked'),
                        $parent = me.$multiSelect.find('#' + parentId),
                        $siblings = me.$checkboxes.filter('[data-parentid=' + parentId + ']'),
                        $siblingschecked = $siblings.filter(':checked');
                    $parent.prop('checked', $siblings.length === $siblingschecked.length && checked);
                    updateParent.call($parent.get(0));
                }
            };

            me.$dropdownbtn.on('click', function () {
                if (me.$dropdownlist.hasClass('hidden')) {
                    me.$updownicon.find('span').removeClass('down').addClass('up');
                    me.$dropdownlist.removeClass('hidden');
                }
            });
            me.$selectAll.on('change', function () {
                var $this = $(this);
                if ($this.prop('checked')) {
                    me.$checkboxes.prop('checked', true);
                } else {
                    me.$checkboxes.prop('checked', false);
                }

                me.updateSelectedText();
            });
            me.$checkboxwrappers.on('click', function () {
                var $checkbox = $(this).find('input[type=checkbox]').first(),
                    isChecked = $checkbox.prop('checked');
                $checkbox.prop('checked', !isChecked).change();
            });
            me.$checkboxes.on('change', function () {
                var $this = $(this),
                    isChecked = $this.prop('checked');
                me.$checkboxes.filter('[data-parentid=' + $this.attr('id') + ']').prop('checked', isChecked).change();
                updateParent.call($this.get(0));

                if ($this.attr('data-is-child') === 'true') {
                    options.onSelect.call(this);
                }

                me.updateSelectedText();
            });

            // hide dropdown
            $(document).delegate('html', 'click', function (e) {
                var $target = $(e.target);
                if (!$target.closest('.multiselect-treeview').length
                    && !$target.parent().hasClass('multiselect-treeview')
                    && !me.$dropdownlist.hasClass('hidden')) {
                    me.$updownicon.find('span').removeClass('up').addClass('down');
                    me.$dropdownlist.addClass('hidden');
                }
            });
        }
    };
})(jQuery);