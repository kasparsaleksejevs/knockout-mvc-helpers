var KnockMvc;
(function (KnockMvc) {
    var DTO;
    (function (DTO) {
        var CustomClassData = /** @class */ (function () {
            function CustomClassData(p) {
                this.SomeInteger = ko.observable();
                this.NiceDouble = ko.observable();
                if (p) {
                    this.SomeInteger(p.SomeInteger);
                    this.NiceDouble(p.NiceDouble);
                }
            }
            return CustomClassData;
        }());
        DTO.CustomClassData = CustomClassData;
        var OtherCustomData = /** @class */ (function () {
            function OtherCustomData(p) {
                this.SomeIntValue = ko.observable();
                this.SomeDoubleValue = ko.observable();
                this.SomeString = ko.observable();
                this.SomeDate = ko.observable();
                this.Kek = ko.observable();
                if (p) {
                    this.SomeIntValue(p.SomeIntValue);
                    this.SomeDoubleValue(p.SomeDoubleValue);
                    this.SomeString(p.SomeString);
                    this.SomeDate(p.SomeDate);
                    this.Kek(p.Kek);
                }
            }
            return OtherCustomData;
        }());
        DTO.OtherCustomData = OtherCustomData;
        var MyRow = /** @class */ (function () {
            function MyRow(p) {
                this.IntValue = ko.observable();
                this.DoubleValue = ko.observable();
                this.DecimalValue = ko.observable();
                this.NullableDecimalValue = ko.observable();
                this.StringValue = ko.observable();
                this.BoolValue = ko.observable();
                this.DateValue = ko.observable();
                this.EnumValue = ko.observable();
                if (p) {
                    this.IntValue(p.IntValue);
                    this.DoubleValue(p.DoubleValue);
                    this.DecimalValue(p.DecimalValue);
                    this.NullableDecimalValue(p.NullableDecimalValue);
                    this.StringValue(p.StringValue);
                    this.BoolValue(p.BoolValue);
                    this.DateValue(p.DateValue);
                    this.EnumValue(p.EnumValue);
                }
            }
            return MyRow;
        }());
        DTO.MyRow = MyRow;
        var MyEnum;
        (function (MyEnum) {
            MyEnum[MyEnum["Val1"] = 1] = "Val1";
            MyEnum[MyEnum["Val2"] = 2] = "Val2";
        })(MyEnum || (MyEnum = {}));
        var MyEnumText = new Map([
            [MyEnum.Val1, 'Value 1'],
            [MyEnum.Val2, 'Other (Descr)'],
        ]);
    })(DTO = KnockMvc.DTO || (KnockMvc.DTO = {}));
})(KnockMvc || (KnockMvc = {}));
(function (KnockMvc) {
    var DTO;
    (function (DTO) {
        var MyStuff;
        (function (MyStuff) {
            var Person = /** @class */ (function () {
                function Person(p) {
                    this.FirstName = ko.observable();
                    this.Lastname = ko.observable();
                    this.Age = ko.observable();
                    if (p) {
                        this.FirstName(p.FirstName);
                        this.Lastname(p.Lastname);
                        this.Age(p.Age);
                    }
                }
                return Person;
            }());
            MyStuff.Person = Person;
            var RowGeneratorTestData = /** @class */ (function () {
                function RowGeneratorTestData(p) {
                    this.IntValue = ko.observable();
                    this.Person = ko.observable();
                    this.IntArr = ko.observableArray();
                    this.IntList = ko.observableArray();
                    this.NullableIntList = ko.observableArray();
                    this.DoubleValue = ko.observable();
                    this.DecimalValue = ko.observable();
                    this.NullableDecimalValue = ko.observable();
                    this.StringValue = ko.observable();
                    this.BoolValue = ko.observable();
                    this.DateValue = ko.observable();
                    this.EnumValue = ko.observable();
                    if (p) {
                        this.IntValue(p.IntValue);
                        this.Person(new Person(p.Person));
                        (_a = this.IntArr).push.apply(_a, p.IntArr);
                        (_b = this.IntList).push.apply(_b, p.IntList);
                        (_c = this.NullableIntList).push.apply(_c, p.NullableIntList);
                        this.DoubleValue(p.DoubleValue);
                        this.DecimalValue(p.DecimalValue);
                        this.NullableDecimalValue(p.NullableDecimalValue);
                        this.StringValue(p.StringValue);
                        this.BoolValue(p.BoolValue);
                        this.DateValue(p.DateValue);
                        this.EnumValue(p.EnumValue);
                    }
                    var _a, _b, _c;
                }
                return RowGeneratorTestData;
            }());
            MyStuff.RowGeneratorTestData = RowGeneratorTestData;
        })(MyStuff = DTO.MyStuff || (DTO.MyStuff = {}));
    })(DTO = KnockMvc.DTO || (KnockMvc.DTO = {}));
})(KnockMvc || (KnockMvc = {}));
//# sourceMappingURL=TypeScriptGenerator.js.map