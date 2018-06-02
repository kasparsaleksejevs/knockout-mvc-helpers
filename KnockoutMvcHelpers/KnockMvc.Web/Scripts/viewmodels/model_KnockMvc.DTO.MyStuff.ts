module KnockMvc.DTO.MyStuff {
	class Person {
		FirstName = ko.observable<string>();
		Lastname = ko.observable<string>();
		Age = ko.observable<number>();
	}
	class RowGeneratorTestData {
		IntValue = ko.observable<number>();
		Person = ko.observable<string>();
		IntArr = ko.observable<string>();
		IntList = ko.observable<string>();
		NullableIntList = ko.observable<string>();
		DoubleValue = ko.observable<number>();
		DecimalValue = ko.observable<number>();
		NullableDecimalValue = ko.observable<number>();
		StringValue = ko.observable<string>();
		BoolValue = ko.observable<boolean>();
		DateValue = ko.observable<Date>();
		EnumValue = ko.observable<string>();
	}
}
