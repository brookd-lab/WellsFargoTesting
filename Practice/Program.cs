/*
 * 
 * Write a C# method that takes a list of integers and returns the second highest number.
 * Handle edge cases like duplicate numbers, empty lists, or lists with only one element. 
 * Provide the method signature and implementation.
 * 
 * Take your time with this one - I'm looking for clean code, proper error handling, 
 * and consideration of edge cases!
 *
 */

// expect 9
//var output = GetSecondHighestNumber(new List<int>() { 10, 10, 9, 1, 2, 3 });
// expect 3
var output = GetSecondHighestNumber(new List<int>() { 9, 1, 2, 3 });
Console.WriteLine(output);

int GetSecondHighestNumber(List<int> numbers)
{
    if (numbers == null)
        throw new ArgumentNullException("List is null.");
    if (numbers.Distinct().Count() < 2)
        throw new ArgumentException("List must contain at least two distinct numbers.");
    if (numbers.Count == 1)
        return numbers[0];

    int output = numbers.Distinct().OrderByDescending(o => o).Take(2).Min(n => n);
    return output;
}