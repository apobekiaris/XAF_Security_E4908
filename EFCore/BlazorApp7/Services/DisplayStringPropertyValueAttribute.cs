using System.ComponentModel.DataAnnotations;

namespace BlazorApp7.Services;

public class DisplayStringPropertyValueAttribute : StringLengthAttribute {
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        return !IsValid(value) ? new ValidationResult(value?.ToString(),
                new[] { validationContext.MemberName }!)
            : ValidationResult.Success;
    }

    public DisplayStringPropertyValueAttribute() : base(0) {
    }
}