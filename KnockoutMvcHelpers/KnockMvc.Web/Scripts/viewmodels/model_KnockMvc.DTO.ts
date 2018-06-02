module KnockMvc.DTO {
	class CustomClassData {
		SomeInteger = ko.observable<number>();
		NiceDouble = ko.observable<number>();
	}
	class OtherCustomData {
		SomeIntValue = ko.observable<number>();
		SomeDoubleValue = ko.observable<number>();
		SomeString = ko.observable<string>();
		SomeDate = ko.observable<Date>();
		Kek = ko.observable<number>();
	}
	class MyRow {
		IntValue = ko.observable<number>();
		DoubleValue = ko.observable<number>();
		DecimalValue = ko.observable<number>();
		NullableDecimalValue = ko.observable<number>();
		StringValue = ko.observable<string>();
		BoolValue = ko.observable<boolean>();
		DateValue = ko.observable<Date>();
		EnumValue = ko.observable<string>();
	}
}
