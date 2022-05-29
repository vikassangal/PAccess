/******************************************************************************
 * Extensions.Collections - ISelector interface
 ******************************************************************************
 * Developers: Russ McClelland (russ.mcclelland@ps.net)
 *             Dmitry Frenkel  (dima.frenkel@ps.net)
 ******************************************************************************
 * Copyright (C) 2004, Perot Systems Corporation. All rights reserved.
 ******************************************************************************/

namespace Peradigm.Framework.Domain.Collections
{
	public interface ISelector
	{
		bool MatchesCriteria( object obj );
	}
}
