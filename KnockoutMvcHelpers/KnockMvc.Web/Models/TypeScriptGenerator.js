var KnockMvc;
(function (KnockMvc) {
    var DTO;
    (function (DTO) {
        var CustomClassData = /** @class */ (function () {
            function CustomClassData() {
                this.SomeInteger = ko.observable();
                this.NiceDouble = ko.observable();
            }
            return CustomClassData;
        }());
        var OtherCustomData = /** @class */ (function () {
            function OtherCustomData() {
                this.SomeIntValue = ko.observable();
                this.SomeDoubleValue = ko.observable();
                this.SomeString = ko.observable();
                this.SomeDate = ko.observable();
                this.Kek = ko.observable();
            }
            return OtherCustomData;
        }());
        var MyRow = /** @class */ (function () {
            function MyRow() {
                this.IntValue = ko.observable();
                this.DoubleValue = ko.observable();
                this.DecimalValue = ko.observable();
                this.NullableDecimalValue = ko.observable();
                this.StringValue = ko.observable();
                this.BoolValue = ko.observable();
                this.DateValue = ko.observable();
                this.EnumValue = ko.observable();
            }
            return MyRow;
        }());
    })(DTO = KnockMvc.DTO || (KnockMvc.DTO = {}));
})(KnockMvc || (KnockMvc = {}));
(function (KnockMvc) {
    var DTO;
    (function (DTO) {
        var MyStuff;
        (function (MyStuff_1) {
            var MyStuff = /** @class */ (function () {
                function MyStuff() {
                    this.FirstName = ko.observable();
                    this.Lastname = ko.observable();
                    this.Age = ko.observable();
                }
                return MyStuff;
            }());
        })(MyStuff = DTO.MyStuff || (DTO.MyStuff = {}));
    })(DTO = KnockMvc.DTO || (KnockMvc.DTO = {}));
})(KnockMvc || (KnockMvc = {}));
//# sourceMappingURL=TypeScriptGenerator.js.map