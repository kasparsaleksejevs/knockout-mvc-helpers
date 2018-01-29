class MyViewModel {

    Rows = ko.observableArray<MyRow>([]);

    constructor() {
        this.load();
    }

    load = () => {
        $.getJSON('/api/Grid', (rows: IMyRow[]) => {
            ko.mapping.fromJS(rows, {}, this.Rows);
        });
    }

    createNew_click = () => {

    }

    edit_click = () => {

    }

    remove_click = () => {

    }

    addRow = () => {
        // we don't know the Id, so Post it
        $.post('api/Grid');
        
    }

    updateRow = (id: number | string) => {
        // we know the Id, so we Put it 
        $.put('api/Grid/' + id);
    }

    removeRow = () => {
        $.delete('api/Grid');
    }

}

ko.applyBindings(new MyViewModel());

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