module KnockMvc.DTO {
    export class CustomClassData {
        public static readonly ConstStr = 'Constant String';
        public static readonly ConstInt = 420;
        public static readonly ReadonlyStaticInt = 4200;
        public readonly ReadonlyStr = 'Readonly String';
        public readonly ReadonlyInt = 42;
        public readonly ReadonlyDate = new Date('2018-10-19T14:49:55');
        public readonly ReadonlyDecNull = null;
        public readonly ReadonlyDecOk = 42.56;
        public readonly ReadonlyBool = true;
        public readonly ReadonlyStringList = ['test1', 'test2', 'test3'];
        public readonly ReadonlyBoolList = [true, false, true, true];
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
                this.SomeDate(new Date(p.SomeDate));
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
                this.DateValue(new Date(p.DateValue));
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

    export enum MyEnum {
        Val1 = 1,
        Val2 = 2,
    }

    export const MyEnumText = new Map<number, string>([
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
                this.DateValue(new Date(p.DateValue));
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

