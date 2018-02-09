class MyRow {
    IntValue = ko.observable<number>();
    DoubleValue = ko.observable<number>();
    DecimalValue = ko.observable<number>();
    StringValue = ko.observable<string>();
    BoolValue = ko.observable<boolean>();
    DateValue = ko.observable<Date>();
    EnumValue = ko.observable<MyEnum>();

    constructor(p?: IMyRow) {
        if (p) {
            this.IntValue(p.IntValue);
            this.DoubleValue(p.DoubleValue);
            this.DecimalValue(p.DecimalValue);
            this.StringValue(p.StringValue);
            this.BoolValue(p.BoolValue);
            this.DateValue(p.DateValue);
            this.EnumValue(p.EnumValue);
        }
        else {
            // default values
            this.IntValue(1);
        }
    }
}

enum MyEnum {
    Val1 = 1,
    Val2 = 2
}

interface IMyRow {
    IntValue: number;
    DoubleValue: number;
    DecimalValue: number;
    StringValue: string;
    BoolValue: boolean;
    DateValue: Date;
    EnumValue: number;
}

class MyViewModel<TRowType> {

    Rows = ko.observableArray<TRowType>([]);
    ModalViewModel = ko.observable<TRowType>();
    deleteUrl: string;
    addOrUpdateUrl: string;
    ignoreRowChangeEffect: boolean;

    constructor(private rowType: new () => TRowType) {
        this.load();
    }

    load = () => {
        $.getJSON('/api/Grid', (rows: IMyRow[]) => {
            this.ignoreRowChangeEffect = true;
            ko.mapping.fromJS(rows, {}, this.Rows);
            this.ignoreRowChangeEffect = false;
        });
    }

    createNew_click = () => {
        this.ModalViewModel(this.getNewRow());
        $('#myModal').modal('show');
    }

    edit_click = (row: TRowType) => {
        this.ModalViewModel(ko.mapping.fromJS(ko.toJS(row)));
        $('#myModal').modal('show');
    }

    remove_click = (row: TRowType) => {
        // + confirmation (?)
        this.removeRow(row);
    }

    addRow = () => {
        // we don't know the Id, so Post it
        ////$.post('api/Grid');

    }

    updateRow = (id: number | string) => {
        // we know the Id, so we Put it 
        ////$.put('api/Grid/' + id);
    }

    removeRow = (row: TRowType) => {
        ////$.delete('api/Grid');
        this.Rows.remove(row);
    }

    rowFadeOut = (node) => {
        if (!this.ignoreRowChangeEffect)
            $(node).css('background-color', '#FFBFBF').fadeOut('slow', function () { $(node).remove(); });
        else
            $(node).remove();
    }

    rowFadeIn = (node) => {
        if (!this.ignoreRowChangeEffect)
            $(node).hide().css('background-color', '#DDFFE6').fadeIn('slow');
    }

    private getNewRow(): TRowType {
        return new this.rowType();
    }
}

ko.applyBindings(new MyViewModel<MyRow>(MyRow));

function ShowWait(parentObj) {
    if (parentObj === undefined)
        parentObj = $('#wrap');

    var objPosition = parentObj.offset();
    $('.wait-modal').css({
        display: 'block',
        top: objPosition.top,
        left: objPosition.left,
        width: parentObj.width() + 'px',
        height: parentObj.height() + 'px'
    });
}

function HideWait() {
    $('.wait-modal').css({
        display: 'none'
    });
}

// === jQuery extensions: =================================

interface JQueryStatic {
    /**
     * Send the data to the server (and receive response) using a HTTP PUT request.
     *
     * @param url A string containing the URL to which the request is sent.
     * @param data A plain object or string that is sent to the server with the request.
     * @param success A callback function that is executed if the request succeeds. Required if dataType is provided, but can be null in that case.
     * @param dataType The type of data expected from the server. Default: Intelligent Guess (xml, json, script, text, html).
     */
    put(url: string, data?: Object | string, success?: (data: any, textStatus: string, jqXHR: JQueryXHR) => any, dataType?: string): JQueryXHR;

    /**
     * Send the data to the server (and receive response) using a HTTP DELETE request.
     *
     * @param url A string containing the URL to which the request is sent.
     * @param data A plain object or string that is sent to the server with the request.
     * @param success A callback function that is executed if the request succeeds. Required if dataType is provided, but can be null in that case.
     * @param dataType The type of data expected from the server. Default: Intelligent Guess (xml, json, script, text, html).
     */
    delete(url: string, data?: Object | string, success?: (data: any, textStatus: string, jqXHR: JQueryXHR) => any, dataType?: string): JQueryXHR;
}

(function ($: JQueryStatic) {

    $.put = function (url: string, data?: Object | string, success?: (data: any, textStatus: string, jqXHR: JQueryXHR) => any, dataType?: string): JQueryXHR {
        return $.ajax({
            url: url,
            type: 'PUT',
            success: success,
            data: data,
            contentType: dataType
        });
    };

    $.delete = function (url: string, data?: Object | string, success?: (data: any, textStatus: string, jqXHR: JQueryXHR) => any, dataType?: string): JQueryXHR {
        return $.ajax({
            url: url,
            type: 'DELETE',
            success: success,
            data: data,
            contentType: dataType
        });
    };
})(jQuery);