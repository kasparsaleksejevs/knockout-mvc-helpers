module KnockMvc.DTO {
    export class CustomClassData {
        SomeInteger = ko.observable<number>();
        NiceDouble = ko.observable<number>();

        constructor(p?: ICustomClassData) {
            if (p) {
                this.SomeInteger(p.SomeInteger);
                this.NiceDouble(p.NiceDouble);
            }
        }
    }

    export interface ICustomClassData {
        SomeInteger: number;
        NiceDouble?: number;
    }

    export class OtherCustomData {
        SomeIntValue = ko.observable<number>();
        SomeDoubleValue = ko.observable<number>();
        SomeString = ko.observable<string>();
        SomeDate = ko.observable<Date>();
        Kek = ko.observable<number>();

        constructor(p?: IOtherCustomData) {
            if (p) {
                this.SomeIntValue(p.SomeIntValue);
                this.SomeDoubleValue(p.SomeDoubleValue);
                this.SomeString(p.SomeString);
                this.SomeDate(p.SomeDate);
                this.Kek(p.Kek);
            }
        }
    }

    export interface IOtherCustomData {
        SomeIntValue: number;
        SomeDoubleValue?: number;
        SomeString: string;
        SomeDate: Date;
        Kek: number;
    }

    export class MyRow {
        IntValue = ko.observable<number>();
        DoubleValue = ko.observable<number>();
        DecimalValue = ko.observable<number>();
        NullableDecimalValue = ko.observable<number>();
        StringValue = ko.observable<string>();
        BoolValue = ko.observable<boolean>();
        DateValue = ko.observable<Date>();
        EnumValue = ko.observable<MyEnum>();

        constructor(p?: IMyRow) {
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
    }

    export interface IMyRow {
        IntValue: number;
        DoubleValue: number;
        DecimalValue: number;
        NullableDecimalValue?: number;
        StringValue: string;
        BoolValue: boolean;
        DateValue: Date;
        EnumValue: number;
    }

    enum MyEnum {
        Val1 = 1,
        Val2 = 2,
    }

    const MyEnumText = new Map<number, string>([
        [MyEnum.Val1, 'Value 1'],
        [MyEnum.Val2, 'Other (Descr)'],
    ]);
}

module KnockMvc.DTO.MyStuff {
    export class Person {
        FirstName = ko.observable<string>();
        Lastname = ko.observable<string>();
        Age = ko.observable<number>();

        constructor(p?: IPerson) {
            if (p) {
                this.FirstName(p.FirstName);
                this.Lastname(p.Lastname);
                this.Age(p.Age);
            }
        }
    }

    export interface IPerson {
        FirstName: string;
        Lastname: string;
        Age: number;
    }

    export class RowGeneratorTestData {
        IntValue = ko.observable<number>();
        Person = ko.observable<Person>();
        IntArr = ko.observableArray<number>();
        IntList = ko.observableArray<number>();
        NullableIntList = ko.observableArray<number>();
        DoubleValue = ko.observable<number>();
        DecimalValue = ko.observable<number>();
        NullableDecimalValue = ko.observable<number>();
        StringValue = ko.observable<string>();
        BoolValue = ko.observable<boolean>();
        DateValue = ko.observable<Date>();
        EnumValue = ko.observable<MyEnum>();

        constructor(p?: IRowGeneratorTestData) {
            if (p) {
                this.IntValue(p.IntValue);
                this.Person(new Person(p.Person));
                this.IntArr.push(...p.IntArr);
                this.IntList.push(...p.IntList);
                this.NullableIntList.push(...p.NullableIntList);
                this.DoubleValue(p.DoubleValue);
                this.DecimalValue(p.DecimalValue);
                this.NullableDecimalValue(p.NullableDecimalValue);
                this.StringValue(p.StringValue);
                this.BoolValue(p.BoolValue);
                this.DateValue(p.DateValue);
                this.EnumValue(p.EnumValue);
            }
        }
    }

    export interface IRowGeneratorTestData {
        IntValue: number;
        Person: IPerson;
        IntArr: number[];
        IntList: number[];
        NullableIntList?: number[];
        DoubleValue: number;
        DecimalValue: number;
        NullableDecimalValue?: number;
        StringValue: string;
        BoolValue: boolean;
        DateValue: Date;
        EnumValue: number;
    }
}

