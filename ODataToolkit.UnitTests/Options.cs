﻿namespace ODataToolkit.UnitTests
{
  using System.Linq;

  using ODataToolkit;

  using Machine.Specifications;

  #region No options

  public class When_not_specifying_any_filter_or_options : Filtering
  {
    private Because of = () => result = concreteCollection.AsQueryable().ExecuteOData();

    private It should_return_all_results = () => result.Count().ShouldEqual(11);
  }

  #endregion

  #region Force dynamic properties

  public class When_using_not_filter_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=not (Age eq 25)", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_three_results = () => result.Count().ShouldEqual(3);
  }

  public class When_using_ge_filter_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=Age ge 25", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_two_results = () => result.Count().ShouldEqual(2);
  }

  public class When_using_gt_filter_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=Age gt 25", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_two_results = () => result.Count().ShouldEqual(1);
  }

  public class When_using_le_filter_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=Age le 25", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_two_results = () => result.Count().ShouldEqual(3);
  }

  public class When_using_lt_filter_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=Age lt 25", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_two_results = () => result.Count().ShouldEqual(2);
  }

  public class When_using_eq_filter_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=Age eq 25", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_one_results = () => result.Count().ShouldEqual(1);
  }

  public class When_using_order_by_on_a_dynamic_type_with_force_dynamic_true : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$orderby=Age", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_four_results = () => result.Count().ShouldEqual(4);
  }

  public class When_using_implicit_boolean_filter : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=IsValid", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_three_results = () => result.Count().ShouldEqual(3);
  }

  public class When_using_implicit_boolean_not_filter : Dynamics
  {
    private Because of = () => result = collection.AsQueryable().ExecuteOData("$filter=not IsValid", dynamicAccessor: OData.DictionaryAccessor);

    private It should_return_one_results = () => result.Count().ShouldEqual(1);
  }

  #endregion

  #region Max page size

  public class When_using_top_3_and_the_page_size_is_5 : PagingAndOrdering
  {
    private Because of = () => result = concreteCollection.AsQueryable().ExecuteOData("?$top=3", maxPageSize: 5);

    private It should_return_three_record = () => result.Count().ShouldEqual(3);

    private It should_start_with_the_first_record = () => result.ElementAt(0).Name.ShouldEqual(concreteCollection.ElementAt(0).Name);

    private It should_then_follow_with_the_second_record = () => result.ElementAt(1).Name.ShouldEqual(concreteCollection.ElementAt(1).Name);

    private It should_then_follow_with_the_third_record = () => result.ElementAt(2).Name.ShouldEqual(concreteCollection.ElementAt(2).Name);
  }

  public class When_using_top_3_and_the_page_size_is_1 : PagingAndOrdering
  {
    private Because of = () => result = concreteCollection.AsQueryable().ExecuteOData("?$top=3", maxPageSize: 1);

    private It should_return_one_record = () => result.Count().ShouldEqual(1);

    private It should_start_with_the_first_record = () => result.ElementAt(0).Name.ShouldEqual(concreteCollection.ElementAt(0).Name);
  }

  public class When_using_an_empty_filter_and_page_size_is_1 : PagingAndOrdering
  {
    private Because of = () => result = concreteCollection.AsQueryable().ExecuteOData(maxPageSize: 1);

    private It should_return_one_record = () => result.Count().ShouldEqual(1);

    private It should_start_with_the_first_record = () => result.ElementAt(0).Name.ShouldEqual(concreteCollection.ElementAt(0).Name);
  }

  #endregion
}
