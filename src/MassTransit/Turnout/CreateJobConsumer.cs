﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Turnout
{
    using System.Threading.Tasks;

    /// <summary>
    /// The consumer that creates the job using the turnout host
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreateJobConsumer<T> :
        IConsumer<T>
        where T : class

    {
        readonly ITurnoutController _turnoutHost;
        readonly IJobFactory<T> _jobFactory;

        public CreateJobConsumer(ITurnoutController turnoutHost, IJobFactory<T> jobFactory)
        {
            _turnoutHost = turnoutHost;
            _jobFactory = jobFactory;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            var job = await _turnoutHost.CreateJob(context, _jobFactory).ConfigureAwait(false);
        }
    }
}