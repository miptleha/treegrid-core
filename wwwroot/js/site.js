$('#apply').click(function () {
    var opt = {};
    $('#options [type=checkbox]').each(function () {
        if (!$(this).prop('checked'))
            opt[$(this).attr('name')] = false;
    });
    updateTables(opt);
});

function updateTables(opt) {
    var tg = new TreeGrid($.extend({}, opt, { target: $('#grid1'), id: 'gdp', url: window.data_url }));
    tg.init();

    tg = new TreeGrid($.extend({}, opt, { target: $('#grid2'), id: 'employees', url: window.data_url }));
    tg.init();

    tg = new TreeGrid($.extend({}, opt, { target: $('#grid3'), id: 'hierarchy', url: window.data_url }));
    tg.init();
}