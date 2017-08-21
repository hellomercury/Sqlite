using System;
using System.Collections.Generic;
using System.Linq;

public class SQLiteException : Exception
{
    public SQLite3.Result Result { get; private set; }

    protected SQLiteException(SQLite3.Result r, string message) : base(message)
    {
        Result = r;
    }

    public static SQLiteException New(SQLite3.Result r, string message)
    {
        return new SQLiteException(r, message);
    }
}

public class NotNullConstraintViolationException : SQLiteException
{
    public IEnumerable<TableMapping.Column> Columns { get; protected set; }

    protected NotNullConstraintViolationException(SQLite3.Result r, string message)
        : this(r, message, null, null)
    {

    }

    protected NotNullConstraintViolationException(SQLite3.Result r, string message, TableMapping mapping, object obj)
        : base(r, message)
    {
        if (mapping != null && obj != null)
        {
            Columns = from c in mapping.Columns
                           where c.IsNullable == false && c.GetValue(obj) == null
                           select c;
        }
    }

    public new static NotNullConstraintViolationException New(SQLite3.Result r, string message)
    {
        return new NotNullConstraintViolationException(r, message);
    }

    public static NotNullConstraintViolationException New(SQLite3.Result r, string message, TableMapping mapping, object obj)
    {
        return new NotNullConstraintViolationException(r, message, mapping, obj);
    }

    public static NotNullConstraintViolationException New(SQLiteException exception, TableMapping mapping, object obj)
    {
        return new NotNullConstraintViolationException(exception.Result, exception.Message, mapping, obj);
    }
}

